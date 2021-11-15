﻿/*
Technitium DNS Server
Copyright (C) 2021  Shreyas Zare (shreyas@technitium.com)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using DnsServerCore.ApplicationCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using TechnitiumLibrary.Net;
using TechnitiumLibrary.Net.Dns;

namespace Failover
{
    class HealthService : IDisposable
    {
        #region variables

        static HealthService _healthService;

        readonly IDnsServer _dnsServer;

        readonly ConcurrentDictionary<string, HealthCheck> _healthChecks = new ConcurrentDictionary<string, HealthCheck>(1, 5);
        readonly ConcurrentDictionary<string, EmailAlert> _emailAlerts = new ConcurrentDictionary<string, EmailAlert>(1, 2);
        readonly ConcurrentDictionary<string, WebHook> _webHooks = new ConcurrentDictionary<string, WebHook>(1, 2);
        readonly ConcurrentDictionary<NetworkAddress, bool> _underMaintenance = new ConcurrentDictionary<NetworkAddress, bool>();

        readonly ConcurrentDictionary<string, HealthMonitor> _healthMonitors = new ConcurrentDictionary<string, HealthMonitor>();

        readonly Timer _maintenanceTimer;
        const int MAINTENANCE_TIMER_INTERVAL = 15 * 60 * 1000; //15 mins

        #endregion

        #region constructor

        private HealthService(IDnsServer dnsServer)
        {
            _dnsServer = dnsServer;

            _maintenanceTimer = new Timer(delegate (object state)
            {
                try
                {
                    foreach (KeyValuePair<string, HealthMonitor> healthMonitor in _healthMonitors)
                    {
                        if (healthMonitor.Value.IsExpired())
                        {
                            if (_healthMonitors.TryRemove(healthMonitor.Key, out HealthMonitor removedMonitor))
                                removedMonitor.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _dnsServer.WriteLog(ex);
                }
                finally
                {
                    if (!_disposed)
                        _maintenanceTimer.Change(MAINTENANCE_TIMER_INTERVAL, Timeout.Infinite);
                }
            }, null, Timeout.Infinite, Timeout.Infinite);

            _maintenanceTimer.Change(MAINTENANCE_TIMER_INTERVAL, Timeout.Infinite);
        }

        #endregion

        #region IDisposable

        bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                foreach (KeyValuePair<string, HealthCheck> healthCheck in _healthChecks)
                    healthCheck.Value.Dispose();

                _healthChecks.Clear();

                foreach (KeyValuePair<string, EmailAlert> emailAlert in _emailAlerts)
                    emailAlert.Value.Dispose();

                _emailAlerts.Clear();

                foreach (KeyValuePair<string, WebHook> webHook in _webHooks)
                    webHook.Value.Dispose();

                _webHooks.Clear();

                foreach (KeyValuePair<string, HealthMonitor> healthMonitor in _healthMonitors)
                    healthMonitor.Value.Dispose();

                _healthMonitors.Clear();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region static

        public static HealthService Create(IDnsServer dnsServer)
        {
            if (_healthService is null)
                _healthService = new HealthService(dnsServer);

            return _healthService;
        }

        #endregion

        #region private

        private static string GetHealthMonitorKey(IPAddress address, string healthCheck, Uri healthCheckUrl)
        {
            //key: health-check|127.0.0.1
            //key: health-check|127.0.0.1|http://example.com/

            if (healthCheckUrl is null)
                return healthCheck + "|" + address.ToString();
            else
                return healthCheck + "|" + address.ToString() + "|" + healthCheckUrl.AbsoluteUri;
        }

        private static string GetHealthMonitorKey(string domain, DnsResourceRecordType type, string healthCheck, Uri healthCheckUrl)
        {
            //key: health-check|example.com|A
            //key: health-check|example.com|AAAA|http://example.com/

            if (healthCheckUrl is null)
                return healthCheck + "|" + domain + "|" + type.ToString();
            else
                return healthCheck + "|" + domain + "|" + type.ToString() + "|" + healthCheckUrl.AbsoluteUri;
        }

        private void RemoveHealthMonitor(string healthCheck)
        {
            foreach (KeyValuePair<string, HealthMonitor> healthMonitor in _healthMonitors)
            {
                if (healthMonitor.Key.StartsWith(healthCheck + "|"))
                {
                    if (_healthMonitors.TryRemove(healthMonitor.Key, out HealthMonitor removedMonitor))
                        removedMonitor.Dispose();
                }
            }
        }

        #endregion

        #region public

        public void Initialize(dynamic jsonConfig)
        {
            //email alerts
            {
                //add or update email alerts
                foreach (dynamic jsonEmailAlert in jsonConfig.emailAlerts)
                {
                    string name;

                    if (jsonEmailAlert.name is null)
                        name = "default";
                    else
                        name = jsonEmailAlert.name.Value;

                    if (_emailAlerts.TryGetValue(name, out EmailAlert existingEmailAlert))
                    {
                        //update
                        existingEmailAlert.Reload(jsonEmailAlert);
                    }
                    else
                    {
                        //add
                        EmailAlert emailAlert = new EmailAlert(this, jsonEmailAlert);

                        _emailAlerts.TryAdd(emailAlert.Name, emailAlert);
                    }
                }

                //remove email alerts that dont exists in config
                foreach (KeyValuePair<string, EmailAlert> emailAlert in _emailAlerts)
                {
                    bool emailAlertExists = false;

                    foreach (dynamic jsonEmailAlert in jsonConfig.emailAlerts)
                    {
                        string name;

                        if (jsonEmailAlert.name is null)
                            name = "default";
                        else
                            name = jsonEmailAlert.name.Value;

                        if (name == emailAlert.Key)
                        {
                            emailAlertExists = true;
                            break;
                        }
                    }

                    if (!emailAlertExists)
                    {
                        if (_emailAlerts.TryRemove(emailAlert.Key, out EmailAlert removedEmailAlert))
                            removedEmailAlert.Dispose();
                    }
                }
            }

            //web hooks
            {
                //add or update email alerts
                foreach (dynamic jsonWebHook in jsonConfig.webHooks)
                {
                    string name;

                    if (jsonWebHook.name is null)
                        name = "default";
                    else
                        name = jsonWebHook.name.Value;

                    if (_webHooks.TryGetValue(name, out WebHook existingWebHook))
                    {
                        //update
                        existingWebHook.Reload(jsonWebHook);
                    }
                    else
                    {
                        //add
                        WebHook webHook = new WebHook(this, jsonWebHook);

                        _webHooks.TryAdd(webHook.Name, webHook);
                    }
                }

                //remove email alerts that dont exists in config
                foreach (KeyValuePair<string, WebHook> webHook in _webHooks)
                {
                    bool webHookExists = false;

                    foreach (dynamic jsonWebHook in jsonConfig.webHooks)
                    {
                        string name;

                        if (jsonWebHook.name is null)
                            name = "default";
                        else
                            name = jsonWebHook.name.Value;

                        if (name == webHook.Key)
                        {
                            webHookExists = true;
                            break;
                        }
                    }

                    if (!webHookExists)
                    {
                        if (_webHooks.TryRemove(webHook.Key, out WebHook removedWebHook))
                            removedWebHook.Dispose();
                    }
                }
            }

            //health checks
            {
                //add or update health checks
                foreach (dynamic jsonHealthCheck in jsonConfig.healthChecks)
                {
                    string name;

                    if (jsonHealthCheck.name is null)
                        name = "default";
                    else
                        name = jsonHealthCheck.name.Value;

                    if (_healthChecks.TryGetValue(name, out HealthCheck existingHealthCheck))
                    {
                        //update
                        existingHealthCheck.Reload(jsonHealthCheck);
                    }
                    else
                    {
                        //add
                        HealthCheck healthCheck = new HealthCheck(this, jsonHealthCheck);

                        _healthChecks.TryAdd(healthCheck.Name, healthCheck);
                    }
                }

                //remove health checks that dont exists in config
                foreach (KeyValuePair<string, HealthCheck> healthCheck in _healthChecks)
                {
                    bool healthCheckExists = false;

                    foreach (dynamic jsonHealthCheck in jsonConfig.healthChecks)
                    {
                        string name;

                        if (jsonHealthCheck.name is null)
                            name = "default";
                        else
                            name = jsonHealthCheck.name.Value;

                        if (name == healthCheck.Key)
                        {
                            healthCheckExists = true;
                            break;
                        }
                    }

                    if (!healthCheckExists)
                    {
                        if (_healthChecks.TryRemove(healthCheck.Key, out HealthCheck removedHealthCheck))
                        {
                            //remove health monitors using this health check
                            RemoveHealthMonitor(healthCheck.Key);

                            removedHealthCheck.Dispose();
                        }
                    }
                }
            }

            //under maintenance networks
            _underMaintenance.Clear();

            if (jsonConfig.underMaintenance is not null)
            {
                foreach (dynamic jsonNetwork in jsonConfig.underMaintenance)
                {
                    string network = jsonNetwork.network.Value;
                    bool enable = jsonNetwork.enable.Value;

                    _underMaintenance.TryAdd(NetworkAddress.Parse(network), enable);
                }
            }
        }

        public HealthCheckResponse QueryStatus(IPAddress address, string healthCheck, Uri healthCheckUrl, bool tryAdd)
        {
            string healthMonitorKey = GetHealthMonitorKey(address, healthCheck, healthCheckUrl);

            if (_healthMonitors.TryGetValue(healthMonitorKey, out HealthMonitor monitor))
                return monitor.LastHealthCheckResponse;

            if (_healthChecks.TryGetValue(healthCheck, out HealthCheck existingHealthCheck))
            {
                if (tryAdd)
                {
                    monitor = new HealthMonitor(_dnsServer, address, existingHealthCheck, healthCheckUrl);

                    if (!_healthMonitors.TryAdd(healthMonitorKey, monitor))
                        monitor.Dispose(); //failed to add first
                }

                return new HealthCheckResponse(HealthStatus.Unknown);
            }
            else
            {
                return new HealthCheckResponse(HealthStatus.Failed, "No such health check: " + healthCheck);
            }
        }

        public HealthCheckResponse QueryStatus(string domain, DnsResourceRecordType type, string healthCheck, Uri healthCheckUrl, bool tryAdd)
        {
            domain = domain.ToLower();

            string healthMonitorKey = GetHealthMonitorKey(domain, type, healthCheck, healthCheckUrl);

            if (_healthMonitors.TryGetValue(healthMonitorKey, out HealthMonitor monitor))
                return monitor.LastHealthCheckResponse;

            if (_healthChecks.TryGetValue(healthCheck, out HealthCheck existingHealthCheck))
            {
                if (tryAdd)
                {
                    monitor = new HealthMonitor(_dnsServer, domain, type, existingHealthCheck, healthCheckUrl);

                    if (!_healthMonitors.TryAdd(healthMonitorKey, monitor))
                        monitor.Dispose(); //failed to add first
                }

                return new HealthCheckResponse(HealthStatus.Unknown);
            }
            else
            {
                return new HealthCheckResponse(HealthStatus.Failed, "No such health check: " + healthCheck);
            }
        }

        #endregion

        #region properties

        public IReadOnlyDictionary<string, HealthCheck> HealthChecks
        { get { return _healthChecks; } }

        public IReadOnlyDictionary<string, EmailAlert> EmailAlerts
        { get { return _emailAlerts; } }

        public IReadOnlyDictionary<string, WebHook> WebHooks
        { get { return _webHooks; } }

        public IReadOnlyDictionary<NetworkAddress, bool> UnderMaintenance
        { get { return _underMaintenance; } }

        public IDnsServer DnsServer
        { get { return _dnsServer; } }

        #endregion
    }
}
