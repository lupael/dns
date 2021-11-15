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
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TechnitiumLibrary.Net.Dns;
using TechnitiumLibrary.Net.Dns.ResourceRecords;

namespace GeoContinent
{
    public sealed class CNAME : IDnsApplication, IDnsAppRecordRequestHandler
    {
        #region variables

        MaxMind _maxMind;

        #endregion

        #region IDisposable

        bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_maxMind is not null)
                    _maxMind.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region public

        public Task InitializeAsync(IDnsServer dnsServer, string config)
        {
            _maxMind = MaxMind.Create(dnsServer);

            return Task.CompletedTask;
        }

        public Task<DnsDatagram> ProcessRequestAsync(DnsDatagram request, IPEndPoint remoteEP, DnsTransportProtocol protocol, bool isRecursionAllowed, string zoneName, uint appRecordTtl, string appRecordData)
        {
            dynamic jsonAppRecordData = JsonConvert.DeserializeObject(appRecordData);
            dynamic jsonContinent;

            if (_maxMind.DatabaseReader.TryCountry(remoteEP.Address, out CountryResponse response))
            {
                jsonContinent = jsonAppRecordData[response.Continent.Code];
                if (jsonContinent == null)
                    jsonContinent = jsonAppRecordData["default"];
            }
            else
            {
                jsonContinent = jsonAppRecordData["default"];
            }

            if (jsonContinent == null)
                return Task.FromResult<DnsDatagram>(null);

            string cname = jsonContinent.Value;
            if (string.IsNullOrEmpty(cname))
                return Task.FromResult<DnsDatagram>(null);

            IReadOnlyList<DnsResourceRecord> answers;

            if (request.Question[0].Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase)) //check for zone apex
                answers = new DnsResourceRecord[] { new DnsResourceRecord(request.Question[0].Name, DnsResourceRecordType.ANAME, DnsClass.IN, appRecordTtl, new DnsANAMERecord(cname)) }; //use ANAME
            else
                answers = new DnsResourceRecord[] { new DnsResourceRecord(request.Question[0].Name, DnsResourceRecordType.CNAME, DnsClass.IN, appRecordTtl, new DnsCNAMERecord(cname)) };

            return Task.FromResult(new DnsDatagram(request.Identifier, true, request.OPCODE, true, false, request.RecursionDesired, isRecursionAllowed, false, false, DnsResponseCode.NoError, request.Question, answers));
        }

        #endregion

        #region properties

        public string Description
        { get { return "Returns CNAME record based on the continent the client queries from using MaxMind GeoIP2 Country database. Note that the app will return ANAME record for an APP record at zone apex. Use the two character continent code like \"NA\" (North America) or \"OC\" (Oceania)."; } }

        public string ApplicationRecordDataTemplate
        {
            get
            {
                return @"{
  ""EU"": ""eu.example.com"",
  ""default"": ""example.com""
}";
            }
        }

        #endregion
    }
}
