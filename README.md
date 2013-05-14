PeachFarmer
===========

PeachFarmer is a utility to collect log files from remote machines performing fuzz testing with the [Peach Fuzzing Framework](http://www.peachfuzzer.com).

This is useful if you're running Peach instances on several AWS instances and you don't want to have to keep logging into each one to collect the logs and aggregate them manually.

PeachFarmerClient - Run on your local machine to collect results from Peach workers.

RemoteHarvester - Run this on each of your remote worker machines to act as a server to send back log files to the PeachFarmerClient.

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