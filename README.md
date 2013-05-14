PeachFarmer
===========

PeachFarmer is a utility to collect log files from remote machines performing fuzz testing with the [Peach Fuzzing Framework](http://www.peachfuzzer.com).

This is useful if you're running Peach instances on several AWS instances and you don't want to have to keep logging into each one to collect the logs and aggregate them manually.

PeachFarmerClient - Run on your local machine to collect results from Peach workers.

RemoteHarvester - Run this on each of your remote worker machines to act as a server to send back log files to the PeachFarmerClient.

# QuickStart

## Simple Usage

On the remote machine (IP adddress=192.168.1.101), launch RemoteHarvester and specify the Peach log folder:

<pre>RemoteHarvester -d=c:\peachlogs\</pre>

On the local machine, specify the remote machine and the aggregate log folder in which to collect log files:

<pre>PeachFarmerClient -t 192.168.1.101 -d=c:\aggregatedlogs\</pre>