<p align="center">
	<a href="https://technitium.com/dns/">
		<img src="https://technitium.com/img/logo.png" alt="Technitium DNS Server" /><br />
		<b>Technitium DNS Server</b>
	</a><br />
	<br />
	<b>Self host a DNS server for privacy & security</b><br />
	<b>Block ads & malware at DNS level for your entire network!</b>
</p>
<p align="center">
<img src="https://technitium.com/dns/ScreenShot1.png" alt="Technitium DNS Server" />
</p>

Technitium DNS Server is an open source authoritative as well as recursive DNS server that can be used for self hosting a DNS server for privacy & security. It works out-of-the-box with no or minimal configuration and provides a user friendly web console accessible using any modern web browser.

Nobody really bothers about domain name resolution since it works automatically behind the scenes and is complex to understand. Most computer software use the operating system's DNS resolver that usually query the configured ISP's DNS server using UDP protocol. This way works well for most people but, your ISP can see and control what website you can visit even when the website employ HTTPS security. Not only that, some ISPs can redirect, block or inject content into websites you visit even when you use a different DNS provider like Google DNS or Cloudflare DNS. Having Technitium DNS Server configured to use [DNS-over-TLS](https://en.wikipedia.org/wiki/DNS_over_TLS) or [DNS-over-HTTPS](https://en.wikipedia.org/wiki/DNS_over_HTTPS) forwarders, these privacy & security issues can be mitigated very effectively.

Be it a home network or an organization's network, having a locally running DNS server gives you more insights into your network and helps to understand it better using the DNS logs and stats. It improves overall performance since most queries are served from the DNS cache making web sites load faster by not having to wait for frequent DNS resolutions. It also gives you an additional control over your network allowing you to block domain names network wide and also allows you to route your DNS traffic securely using encrypted DNS protocols.

# Features
- Works on Windows, Linux, macOS and Raspberry Pi.
- Docker image available on [Docker Hub](https://hub.docker.com/r/technitium/dns-server).
- Installs in just a minute and works out-of-the-box with zero configuration.
- Block ads & malware using one or more block list URLs.
- High performance DNS server that can serve millions of requests per minute even on a commodity desktop PC hardware (load tested on Intel i7-8700 CPU with more than 100,000 request/second).
- Self host [DNS-over-TLS](https://en.wikipedia.org/wiki/DNS_over_TLS) and [DNS-over-HTTPS](https://en.wikipedia.org/wiki/DNS_over_HTTPS) DNS service on your network.
- Use public DNS resolvers like Cloudflare, Google & Quad9 with [DNS-over-TLS](https://en.wikipedia.org/wiki/DNS_over_TLS) and [DNS-over-HTTPS](https://en.wikipedia.org/wiki/DNS_over_HTTPS) protocols as forwarders.
- Advance caching with features like serve stale, prefetching and auto prefetching.
- Supports working as an authoritative as well as a recursive DNS server.
- CNAME cloaking feature to block domain names that resolve to CNAME which are blocked.
- QNAME minimization support in recursive resolver [draft-ietf-dnsop-rfc7816bis-04](https://datatracker.ietf.org/doc/html/draft-ietf-dnsop-rfc7816bis-04).
- QNAME randomization support for UDP transport protocol [draft-vixie-dnsext-dns0x20-00](https://datatracker.ietf.org/doc/html/draft-vixie-dnsext-dns0x20-00).
- DNAME record [RFC 6672](https://datatracker.ietf.org/doc/html/rfc6672) support.
- ANAME propriety record support to allow using CNAME like feature at zone apex.
- APP propriety record support that allows custom DNS Apps to directly handle DNS requests and return a custom DNS response based on any business logic.
- Support for features like Split Horizon and Geolocation based responses using DNS Apps feature.
- Support for REGEX based block lists with different block lists for different client IP addresses or subnet using Advanced Blocking DNS App.
- Primary, Secondary, Stub, and Conditional Forwarder zone support.
- Zone transfer over TLS (XFR-over-TLS) [draft-ietf-dprive-xfr-over-tls](https://datatracker.ietf.org/doc/draft-ietf-dprive-xfr-over-tls/) support.
- Secret key transaction authentication (TSIG) [RFC 8945](https://datatracker.ietf.org/doc/html/rfc8945) support for zone transfers.
- Self host your domain names on your own DNS server.
- Wildcard sub domain support.
- Enable/disable zones and records to allow testing with ease.
- Built-in DNS Client with option to import responses to local zone.
- Supports out-of-order DNS request processing for DNS-over-TCP and DNS-over-TLS protocols.
- Built-in DHCP Server that can work for multiple networks.
- IPv6 support in DNS server core.
- HTTP & SOCKS5 proxy support which can be configured to route DNS over [Tor Network](https://www.torproject.org/) or use Cloudflare's hidden DNS resolver.
- Web console portal for easy configuration using any web browser.
- Built in HTTP API to allow 3rd party apps to control and configure the DNS server.
- Built-in system logging and query logging.
- Open source cross-platform .NET 5 implementation hosted on GitHub.

# Planned Features
- Multi-user role based access.
- API key to provide long term access token.
- Clustering support to manage two or more DNS servers.
- Dynamic DNS updates.
- EDNS support.
- DNSSEC support.

# Installation
- **Windows**: [Download setup installer](https://download.technitium.com/dns/DnsServerSetup.zip) for easy installation.
- **Linux & Raspberry Pi**: Follow install instructions from [this blog post](https://blog.technitium.com/2017/11/running-dns-server-on-ubuntu-linux.html).
- **Cross-Platform**: [Download portable app](https://download.technitium.com/dns/DnsServerPortable.tar.gz) to run on any platform that has .NET 5 installed.
- **Docker**: Pull the official image from [Docker Hub](https://hub.docker.com/r/technitium/dns-server). Use the [docker-compose.yml](https://github.com/TechnitiumSoftware/DnsServer/blob/master/docker-compose.yml) example to create a new container and edit it as required for your deployments. For more details and troubleshooting read the [install instructions](https://blog.technitium.com/2017/11/running-dns-server-on-ubuntu-linux.html).

# Docker Environment Variables
Technitium DNS Server supports environment variables to allow initializing the config when the DNS server starts for the first time. Read the [environment variable documentation](https://github.com/TechnitiumSoftware/DnsServer/blob/master/DockerEnvironmentVariables.md) for complete details.

# API Documentation
The DNS server HTTP API allows any 3rd party app or script to configure the DNS server. The HTTP API is used by the web console and thus all the actions that the web console does can be performed via the API. Read the [HTTP API documentation](https://github.com/TechnitiumSoftware/DnsServer/blob/master/APIDOCS.md) for complete details.

