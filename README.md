PeachFarmer
===========

## What is It?

PeachFarmer is a utility to collect log files from remote machines performing fuzz testing with the [Peach Fuzzing Framework](http://www.peachfuzzer.com).

This is useful if you're running Peach instances on several AWS instances and you don't want to have to keep logging into each one to collect the logs and aggregate them manually.

PeachFarmerClient - Runs on the user’s local machine and connects to each of the RemoteHarvester servers to retrieve the fuzzing output. The client then aggregates all of the files into a single location on the local machine.

RemoteHarvester - A minimal server that runs on each of the remote fuzzing instances. It listens for connections and provides the client with any new log files the client does not already have.

CertPairGenerator (Windows only) - Simple tool to create self-signed X509 certificates for PeachFarmer to use to communicate over SSL.

## Binaries

### Windows

[PeachFarmer v1.0](https://s3.amazonaws.com/PeachFarmer/v1.0/PeachFarmer.zip)
(MD5: c6c43295568de92a2c0529810e8f7f77)
(SHA-256: b5179103735e6a062fc1bbf2e4ab8f24fe0351e1e0338670a7a3b32faa2d0abe)

### Linux / OS X

Coming soon!

## Requirements

Requires the .NET Framework v. 4.5

## QuickStart

### Simple Usage

On the remote machine (IP adddress=192.168.1.101), launch RemoteHarvester and specify the Peach log folder:

<pre>RemoteHarvester -d c:\peachlogs\</pre>

On the local machine, specify the remote machine and the aggregate log folder in which to collect log files:

<pre>PeachFarmerClient -t 192.168.1.101 -d c:\aggregatedlogs\</pre>

### Pulling from Multiple Machines

To collect files from multiple Peach instances, create a text file containing hostnames or IP addresses (one per line), as below:

<pre>
ec2-87-145-93-15.compute-1.amazonaws.com
192.168.1.105
fuzzinghost42.example.com
</pre>

On each of these machines, launch RemoteHarvester as below:

<pre>RemoteHarvester -d c:\peachlogs\</pre>

On the local machine, specify the list of hosts and the aggregate log folder in which to collect log files:

<pre>PeachFarmerClient -i hosts.txt -d c:\aggregatedlogs\</pre>

### PeachFarmer over SSL

PeachFarmer supports connections over SSL with mutual authentication (client and server authentication). To transfer files over SSL, launch the RemoteHarvester with the commands below, where server.pfx is the server's X509 certificate and client.pfx is the client's X509 certificate.

<pre>RemoteHarvester -d c:\peachlogs\ --server-cert=server.pfx --client-cert=client.pfx</pre>

On the local machine, use a command similar to the below to communicate with the server over SSL.

<pre>PeachFarmerClient -t 192.168.1.101 -d c:\aggregatedlogs\ --server-cert=server.pfx --client-cert=client.pfx</pre>

## Build Instructions

Builds with Visual Studio 2012 / VS Express for Desktop 2012.

## Disclaimer

iSEC has written this tool and provides it to the community free of charge. While iSEC has designed the tool with security in mind and has performed internal security review of the tool, it has not gone through the same level of rigorous independent review like that of a commercial software product. The software is being provided "as is" without warranty or support. iSEC does not assume liability for any damage caused by use of this tool.