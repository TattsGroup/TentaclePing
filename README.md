TentaclePing is a command line tool that helps to diagnose troubled connections between an Octopus server and a listening Tentacle. 

[Download](https://github.com/OctopusDeploy/TentaclePing/releases)

Unlike regular Ping, TentaclePing makes an SSL connection to the Tentacle service, meaning it will only work if the Tentacle service is running and responding. 

The command takes two arguments:

    TentaclePing <ip/hostname> [<port>]
    
Examples:

    TentaclePing.exe MyServer         # Uses default port (10933)
    TentaclePing.exe 10.0.0.1 10934   # Explicit port

If you are experiencing slow deployments or package uploads with Octopus, you can use TentaclePing to determine whether there is a connectivity problem:

1. Download TentaclePing.zip and extract it on your Octopus server
2. Open a command prompt, and run TentaclePing as shown above
3. Let it run for a few hours. If it encounters failures, it will log them and count the number of failures
4. Send the results to the Octopus Deploy team

If you don't plan to watch TentaclePing run, you may want to pipe the results to a text file:

    TentaclePing.exe MyServer 10933 > Output.txt
    
This file can then easily be sent to the Octopus support team (support@octopusdeploy.com) for analysis.
