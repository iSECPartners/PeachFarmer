PeachFarmer
===========

PeachFarmer is a utility to help collect log files from remote machines performing fuzz testing with Peach.

This is useful if you're running Peach instances on several AWS instances and you don't want to have to keep logging into each one to collect the logs and aggregate them manually.

PeachFarmerClient - Run on your local machine to collect results from Peach workers.

RemoteHarvester - Run this on each of your remote worker machines to act as a server to send back log files to the PeachFarmerClient.

