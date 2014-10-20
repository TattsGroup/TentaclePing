TentaclePing is a command line tool that helps to diagnose troubled connections between an Octopus server and a listening Tentacle. 

[Download](https://github.com/OctopusDeploy/TentaclePing/releases)

Unlike regular Ping, TentaclePing makes an SSL connection to the Tentacle service, meaning it will only work if the Tentacle service is running and responding. 

The command takes two arguments:

    TentaclePing <ip/hostname> [<port>]
    
Examples:

    TentaclePing.exe MyServer         # Uses default port (10933)
    TentaclePing.exe 10.0.0.1 10934   # Explicit port

