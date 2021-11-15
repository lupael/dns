# Technitium DNS Server Change Log

## Version 7.1
Release Date: 23 October 2021
- Added option in settings to automatically configure a self signed certificate for DNS web service.
- Fixed cache poisoning vulnerability reported by Xiang Li, [Network and Information Security Lab, Tsinghua University](https://netsec.ccert.edu.cn/) and Qifan Zhang, [Data-driven Security and Privacy (DSP) Lab, University of California, Irvine](https://faculty.sites.uci.edu/zhouli/research/) when a conditional forwarder zone uses a forwarder controlled by an attacker or uses UDP/TCP forwarder protocol that the attacker can perform MiTM.
- Block Page App: Added support for automatic self signed certificate to allow showing block page for HTTPS websites.
- Drop Requests App: Added option to drop malformed DNS requests.
- Query Logs App: Fixed minor issue which caused the query logs request to fail when a domain with invalid character was logged in the database.
- Advanced Blocking App: Fixed bug in loading regex block list which caused the app to not block the domain names as expected.
- Added logging in DNS server to know why a zone transfer request was refused by the server.
- Added more environment variables for use with Docker to initialize the DNS server config. Read the [environment variable documentation](https://github.com/TechnitiumSoftware/DnsServer/blob/master/DockerEnvironmentVariables.md) for complete details.
- Multiple other minor bug fixes and improvements.

## Version 7.0
Release Date: 2 October 2021
- DNS Apps design updated to allow apps to act as authoritative zones, drop requests, and log queries in addition to the existing APP records in authoritative zones.
- This release is a major update for DNS Apps design and thus any previously installed apps will fail to load after the update. A manual update is required to install the latest app update from the DNS App Store for these apps to work with this new release.
- Advanced Blocking App: This new app allows blocking domain names based on IP address or subnet of the clients by creating groups. It also supports blocking using regex and also supports loading blocked domains from Adblock format lists.
- Block Page App: This new app runs a built-in web server to allow serving a block page to clients when a domain name is blocked.
- Drop Requests App: This new app allows dropping requests that match the blocked questions in the config allowing to block DNS amplification attacks that use specific domain name and query types.
- NX Domain App: This new app allows blocking domain names with a NXDOMAIN response.
- Query Logs (Sqlite): This new app allows logging all queries that the DNS server receives into a Sqlite database. The DNS server web panel adds an Query Logs option to allow querying the app for logged data.
- Failover App: Implemented under maintenance feature to indicate if an address is taken down for maintenance.
- Added Ping check option in DHCP scopes to allow detecting if an IP address is already in use before leasing it.
- Added option to allow removing an allocated DHCP lease.
- This release updates many API calls which may cause issues in 3rd party clients if they are not updated before deploying this new version. It is recommended to check the API documentation for changes before deploying this new release.
- Multiple other minor bug fixes and improvements.

## Version 6.4.1
Release Date: 21 August 2021
- Implemented Delegation Revalidation [draft-ietf-dnsop-ns-revalidation-01](https://datatracker.ietf.org/doc/draft-ietf-dnsop-ns-revalidation/) in recursive resolver.
- Fixed issues with DNS-over-TLS due to "dot" ALPN causing SSL handshake to fail when using NextDNS as forwarder.
- Fixed issues in counting total unique clients in dashboard stats. The future data for total clients will be displayed correctly however the bad data since last release can be fixed by deleting '/etc/dns/config/stats/202108*.dstat' files manually.
- Updated allowed list URL implementation to check for domains zone wise so that subdomain names from blocked list URLs too are allowed.
- Updated DNS Failover App to v1.4 to fix implementation issues.
- Multiple other minor bug fixes and improvements.

## Version 6.4
Release Date: 14 August 2021
- Added DNAME record [RFC 6672](https://datatracker.ietf.org/doc/html/rfc6672) support.
- Implemented incremental zone transfer (IXFR) [RFC 1995](https://datatracker.ietf.org/doc/html/rfc1995) support.
- Implemented secret key transaction authentication (TSIG) [RFC 8945](https://datatracker.ietf.org/doc/html/rfc8945) support for zone transfers.
- Implemented zone transfer over TLS (XFR-over-TLS) [draft-ietf-dprive-xfr-over-tls](https://datatracker.ietf.org/doc/draft-ietf-dprive-xfr-over-tls/) support.
- Added advance options in Settings to control TTL values in Cache.
- Added Resync button to force resync Secondary and Stub zones.
- Updated query rate limiting feature to allow limiting requests from the client's subnet.
- Updated SplitHorizon App to support configuring CIDR networks.
- Updated Failover App to fix multiple issues and added feature to auto generate health check URL from APP record domain name or specify the URL in the APP record data.
- Fixed issues with log file rolling when using local time.
- Multiple other minor bug fixes and improvements.
- Updated few API calls which may cause issues in 3rd party clients if they are not updated before deploying this new version.

## Version 6.3
Release Date: 6 June 2021

- Added Failover App in DNS App Store.
- Added comments option to DNS records in Zones.
- Added Recursion ACL support to specify allowed and denied networks that can perform recursion.
- Added Zone Options feature to allow configuring Zone Transfer and Notify settings per zone.
- Added Queries Per Minute (QPM) Limit feature to limit the number of queries being made by an IP address.
- Added feature to specify custom IP addresses for blocked domain names.
- Added feature to temporarily/permanently disable blocking of domain names.
- Added index page for DNS-over-HTTPS (DoH) web service that displays basic configuration information to user when DoH URL is visited using a web browser.
- Fixed multiple issues in QNAME minimization implementation.
- Fixed multiple DNS Client implementation issues.
- Multiple other minor bug fixes and improvements.
- Updated few API calls which may cause issues in 3rd party clients if they are not updated before deploying this new version.

## Version 6.2.3
Release Date: 2 May 2021

- Improved DNS Apps interface to should if updates are available in the installed apps list.
- Updated stats module to truncate daily stats data to optimize memory usage.
- Fixed issue with QNAME minimization caused due to missing check when response contained no answer and no authority.
- Fixed issue in logger which would fail to start in certain conditions.
- Updated DNS Apps to shuffle addresses in response to allow load balancing.

## Version 6.2.2
Release Date: 24 April 2021

- Fixed issues with recursive resolution.
- Fixed issue in parsing AXFR response.
- Fixed missing tags in responses to reflect correct stats on dashboard.
- Fixed issue with web console redirection on saving settings when using a reverse proxy.
- Multiple other minor bug fixes and improvements.

## Version 6.2.1
Release Date: 17 April 2021

- Updated DNS Cache serve stale implementation for better performance.
- Implemented CNAME resolution optimization in DNS Cache and Auth Zone.
- Fixed issue in DNS Cache caused due to missing check of the type of NS record's RDATA causing cache zone to return special cache RDATA record.
- Fixed issue in DNS client caused when response greater than the buffer size is received.

## Version 6.2
Release Date: 11 April 2021

- Fixed critical bug in block list condition check causing server to respond with `RCODE=Refused` when only using Blocked zone.
- Added option to respond with `RCODE=NxDomain` for blocked domains instead of returning `0.0.0.0` address.
- Renamed `NameError` to `NxDomain` to make the terminology clear that the domain does not exists. Dashboard API returns JSON with new terminology so its advised to test your code before updating the server.

## Version 6.1
Release Date: 10 April 2021

- Added DNS App Store feature that list all available apps for quick and easy installation and update.
- Added 'Overwrite' option in Add Record for zones.
- Multiple ANAME record support added.
- Added block list allowed URL feature to prevent domain names from getting added to the block list zone.
- Fixed bug in ZoneTree.
- Fixed bugs in DNS Apps.
- Split Default DNS App into 5 independent apps that are now available on the DNS App Store.
- Fixed issues in DNS Cache and updated code for memory optimization.
- Upgraded all library projects to .NET 5.
- Multiple other minor bug fixes and improvements.

## Version 6.0
Release Date: 13 March 2021

- Updated entire DNS code base to .NET 5 with new Windows installer. This upgrade will improve overall performance on Windows installations.
- Added support for DNS Application (APP) propriety record with DNS Apps feature support. DNS Apps allows creating custom apps by 3rd party using .NET that run on the DNS server allowing the apps to process DNS requests and provide custom DNS response based on any bussiness logic.
- A default DNS app (available to download separately) supports APP records capable of Split Horizon and Geolocation based responses using MaxMind's GeoIP2 City & Country databases.
- Updated dashboard charts to save legend selection state.
- Updated dashboard with Custom date selection option to display stats.
- Added option to configure max stats days in settings.
- Added option to enable/disable QNAME minimization.
- Added delete existing files option in Restore settings.
- Added support to store query stats data to allow DNS cache auto prefetch to refresh cache when DNS server restarts.
- Updated TLS certificate implementation to allow using self signed certificates for web console, DoH, and DoT.
- Added DHCP lease Reserve/Unreserve options to allow quickly reserving lease for clients.
- Updated DHCP reserved lease option to allow overriding client's host name.
- Fixed issues with DNS cache auto prefetch feature.
- Fixed multiple issues in DNS cache.
- Fixed multiple vulnerabilities causing DNS cache poisoning.
- Multiple other minor bug fixes and improvements.

## Version 5.6
Release Date: 2 January 2021

- Updated standalone console app to work on .NET 5 and removing standalone .NET Framework app support. .NET 5 update will boost performance of the DNS server on all platforms.
- Updated DNS and DHCP listener code to use async IO to improve performance.
- Added HTTPS support for web service that provides the web console access.
- Added support to change the web service local addresses.
- Updated the server to allow changing DNS server end points, the web service end points, or enabling DoH or DoT services instantly without need to manually restart the main service. Basically, you do not need to restart the DNS server app at all for applying any kind of settings as all the changes are applied dynamically.
- Added HTTP compression support in the main web service.
- Added HTTP compression for downloading block lists.
- Added option to clear and delete all dashboard stats and auto clean up old stats files from disk
- Added option to delete all log files and auto clean up old log files from disk.
- Added configurable option to disable logging, allow logging in local time, and to change log folder path.
- Added option in settings to define the refresh interval for block lists with a manual option to force refresh all block lists.
- Added support for exporting backup zip file containing selected items like config files, logs, stats, etc. and allow restoring the backup zip file without restarting the main service.
- Fixed multiple issues in DHCP server's DNS record management.
- Fixed bug in DNS server cache prefetching for stub and conditional forwarder zones causing the cached data to be overwritten by the prefetched output from recursive resolution.
- Fixed html encoding issue in web app.
- Added option in web app to list top 1000 clients, top domains and top blocked domains.
- DNS cache serve stale feature made configurable with default serve stale TTL set to 3 days instead of 7 days.
- Fixed issue in recursive resolver to avoid querying root servers when one of the parent zone's name servers exists in DNS cache.
- Breaking changes in the `getDnsSettings` and `setDnsSettings` API calls will require API clients to update the code before updating the DNS server.
- Multiple other minor bug fixes and improvements.

## Version 5.5
Release Date: 14 November 2020

- Added option to specify bootfile name for PXE booting.
- Implemented DHCP vendor specific information option.
- Implemented strict enforcing of exclusion list.
- Fixed bug in DNS initial server name that was caused due to invalid characters in the computer name.
- Added support for additional record processing for SRV records and fixed issues for NS and MX records processing.
- Multiple other minor bug fixes and improvements.

## Version 5.4
Release Date: 18 October 2020

- Implemented QNAME randomization feature [draft-vixie-dnsext-dns0x20](https://datatracker.ietf.org/doc/html/draft-vixie-dnsext-dns0x20-00).
- Fixed bug causing infinite loop in certain conditions when using UDP as transport.
- Fixed bug in DNS cache querying which caused the server to make unneeded queries when performing recursive resolution.
- Added Create PTR Zone option when adding A or AAAA records.
- Fixed issues with DHCP scope selection when using relay agent.
- Implemented changes to allow changing DHCP scope IP allocation from dynamic to reserved and vice versa.
- Updated DHCP scope to allow specifying Next Server Address for use with TFTP for booting.
- Multiple other minor bug fixes and improvements.

## Version 5.3
Release Date: 26 September 2020

- Fixed issues with DHCP server that caused it to not work correctly with relay agents.
- Updated DHCP server to support multiple scopes to work on a single network interface allowing it to provide different options for groups of devices.
- Multiple other minor bug fixes and improvements.

## Version 5.2
Release Date: 6 September 2020

- Added feature to allow using `certbot` to renew TLS certificates automatically when using DNS-over-HTTPS and DNS-over-TLS.
- Fixed issue in DHCP server that caused thread to block by implementing async methods.
- Fixed bug in DNS client that caused QTYPE mismatch due to QNAME minimization.
- Fixed issues in DNS-over-HTTPS client related to retries and http error handling.
- Multiple other minor bug fixes and improvements.

## Version 5.1
Release Date: 29 August 2020

- Implemented async IO to allow the DNS server handle much higher concurrent loads.
- Implemented independent thread pools for DNS web service and recursive resolver.
- Fixed bug in block list downloader that caused 0 byte file downloads.
- Fixed bug in DHCP server in creating reverse zone.
- Multiple other minor bug fixes and improvements.

## Version 5.0.2
Release Date: 18 July 2020

- Fixed issue of missing port for "This Server" in DNS Client.
- Added domain name that was blocked in the TXT record.
- Fixed bugs in CNAME cloaking implementation.
- Upgraded .NET Framework version to v4.8.
- Multiple other minor bug fixes and improvements.

## Version 5.0.1
Release Date: 6 July 2020

- Fixed serialization bug for TXT records.
- Fixed issue with reading DnsDatagram for DoH POST requests.
- Fixed bug in json serialization of DnsDatagram for DoH json format.
- Fixed bug in RTT calculation for DoH json Connection.

## Version 5.0
Release Date: 4 July 2020

- DNS Server local end points support to allow specifying alternate ports for UDP and TCP protocols.
- DNS Server performance issues caused by thread contention fixed.
- CNAME cloaking implemented to block domain names that resolve to CNAME which are blocked.
- New Block List zone implementation that uses very less memory allowing to load block lists with millions of domain names even on a Raspberry Pi with 1GB RAM.
- QNAME minimization support in recursive resolver [draft-ietf-dnsop-rfc7816bis-04](https://datatracker.ietf.org/doc/html/draft-ietf-dnsop-rfc7816bis-04).
- ANAME propriety record support to allow using CNAME like feature at zone root.
- Added primary zones with NOTIFY implementation [RFC 1996](https://datatracker.ietf.org/doc/html/rfc1996).
- Added secondary zones with NOTIFY implementation [RFC 1996](https://datatracker.ietf.org/doc/html/rfc1996).
- Added stub zones with feature to override records.
- Added conditional forwarder zones with all protocols including DNS-over-HTTPS and DNS-over-TLS support.
- Conditional forwarder zones with feature to override records.
- Conditional forwarder zones with support for multiple forwarders with different sub domain names.
- ByteTree based zone tree implementation which is a complete lock-less and thread safe tree allowing concurrent read and write operations.
- Fixed bug in parsing large TXT records.
- DNS Client with internal support for concurrent querying. This allows querying multiple forwarders simultaneously to return fastest response of all.
- DNS Client with support to import records via zone transfer.
- Multiple other bug fixes in DNS and DHCP modules.
