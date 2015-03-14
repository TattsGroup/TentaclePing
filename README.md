TentaclePing is a command line tool that helps to diagnose troubled connections between an Octopus server and a listening Tentacle. 

[Download](https://github.com/OctopusDeploy/TentaclePing/releases)

## Ping

Unlike regular Ping, TentaclePing makes an SSL connection to the Tentacle service, meaning it will only work if the Tentacle service is running and responding. 

The command takes four arguments:

    OctoPing ping /host=<ip/hostname> [/port=<port>] [/datasize=<datasize>] [/chunksize=<chunksize>] [/nossl]
    
Examples:

    OctoPing.exe ping /host=MyServer               # Uses default port (10933)
    OctoPing.exe ping /host=10.0.0.1 /port=10934   # Explicit port

If you are experiencing slow deployments or package uploads with Octopus, you can use TentaclePing to determine whether there is a connectivity problem:

1. Download TentaclePing.zip and extract it on your Octopus server
2. Open a command prompt, and run TentaclePing as shown above
3. Let it run for a few hours. If it encounters failures, it will log them and count the number of failures
4. Send the results to the Octopus Deploy team

## Pong

Normally, TentaclePing is pointed at a listening Tentacle or Octopus Server polling endpoint. If you have run TentaclePing and determinied that you have issues with connections being dropped, the next step is to determine whether the problem is with the Tentacle/Octopus server, or just a network issue. TentaclePong is the server equivalent for TentaclePing, listening on a port and waiting for requests from TentaclePing. 

Usage:

    OctoPing pong /port=10945              		                                # On server 1, listen on a port
    OctoPing pong /host=YourServer /port=10945   		                        # Server 2 will ping server 1
    OctoPing pong /host=YourServer /port=10945 /datasize=100		            # Server 2 will send 100Mb of data using default chunk size (2Mb) to server 1
    OctoPing pong /host=YourServer /port=10945 /datasize=100 /chunksize=10  	# Server 2 will send 100Mb of data using a chunk size of 10Mb to server 1


If TentaclePing reports problems when pointed at a listening Tentacle/Octopus server, but there are no problems with TentaclePong, then the issue is most likely a bug or resource utilization problem on the Tentacle/Octopus server (e.g., high CPU usage, limited memory, etc.). 

However, if the connection problems persist with TentaclePong, then the problem is most likely down to a network issue. Unfortunately there's really not much the Octopus team can do to help with that. 

## Capturing output

If you don't plan to watch TentaclePing run, you may want to pipe the results to a text file:

    OctoPing.exe ping /host=MyServer /port=10933 > Output.txt
    
This file can then easily be sent to the Octopus support team (support@octopusdeploy.com) for analysis.
