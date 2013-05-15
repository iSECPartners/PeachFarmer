PeachFarmer
===========

PeachFarmer facilitates fuzz testing in the cloud. PeachFarmer is designed to be used in conjunction with the [Peach fuzzing framework](http://www.peachfuzzer.com). Peach allows the user to split up a fuzzing job among many machines, but does not offer a built-in way to gather the logs and crash dumps from all these separate machines. PeachFarmer offers a way for users to collect all of their fuzzing results both quickly and easily.

* __PeachFarmerClient__ - Runs on the user’s local machine and connects to each of the RemoteHarvester servers to retrieve the fuzzing output. The client then aggregates all of the files into a single location on the local machine.

* __RemoteHarvester__ - A minimal server that runs on each of the remote fuzzing instances. It listens for connections and provides the client with any new log files the client does not already have.

* __CertPairGenerator__ (Windows only) - Simple tool to create self-signed X509 certificates for PeachFarmer to use to communicate over SSL.

## Binaries

### Windows

[PeachFarmer v1.0](https://s3.amazonaws.com/PeachFarmer/v1.0/PeachFarmer.zip)  
(MD5: c6c43295568de92a2c0529810e8f7f77)  
(SHA-256: b5179103735e6a062fc1bbf2e4ab8f24fe0351e1e0338670a7a3b32faa2d0abe)

### Linux / OS X

Coming soon!

## Usage

See [Usage Instructions](https://github.com/iSECPartners/PeachFarmer/wiki/Usage-Instructions)

## Requirements

Requires the .NET Framework v. 4.5

## Build Instructions

Builds with Visual Studio 2012 / VS Express for Desktop 2012.

## Disclaimer

iSEC has written this tool and provides it to the community free of charge. While iSEC has designed the tool with security in mind and has performed internal security review of the tool, it has not gone through the same level of rigorous independent review as a commercial software product. The software is being provided "as is" without warranty or support. iSEC does not assume liability for any damage caused by use of this tool.