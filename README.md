# TravelRepublic.DnsClient

A dns client library based on [DnDns](http://dndns.codeplex.com/) for .NET.

[![Build status](https://ci.appveyor.com/api/projects/status/nw3s9bvmwm5ask1c?svg=true)](https://ci.appveyor.com/project/travelrepublic-bot/travelrepublic-dnsclient)

###Install

```
PM> Install-Package TravelRepublic.DnsClient
```

###Get started 

```csharp
var client = new DnsClientBuilder()
	.WithDnsServer(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53))
	.Build();

var response = client.Query("www.google.com", NsType.A, NsClass.INET, ProtocolType.Udp);
var record = (ARecord)response.Answers[0];
```