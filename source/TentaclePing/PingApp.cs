using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using CLAP;

namespace TentaclePing
{
    class PingApp
    {
        private static readonly TimeSpan sendReceiveTimeout = TimeSpan.FromMinutes(30);
        private static int chunkSizeInBytes;

        private static readonly X509Certificate2 serverCertificate = new X509Certificate2(Convert.FromBase64String("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"), (string)null);
        private static bool isStopped;
        private static TcpListener listener;
        private static string response = new string('A', 1024 * 10);

        [Verb]
        public static void Ping(
            [Required]
            string host,
            [DefaultValue(10933)]
            int port,
            int dataSize,
            [DefaultValue(2)]
            int chunkSize,
            [DefaultValue(false),
            Description("Disable SSL")]
            bool noSSL
            )
        {
            var enableSSL = !noSSL;
            
            Console.WriteLine("Pinging " + host + " on port " + port + (dataSize == 0 ? "" : ", sending " + dataSize + "Mb of data in " + chunkSize + "Mb chunks") + (enableSSL ? " With SSL" : " Without SSL"));
            chunkSizeInBytes = 1024 * 1024 * chunkSize;

            ExecutePing(host, port, dataSize, enableSSL);
        }

        private static int ExecutePing(string hostname, int port, int dataSize, bool enableSSL)
        {
            var failCount = 0;
            var successCount = 0;
            while (true)
            {
                var attempts = successCount + failCount;
                if (attempts > 0 && attempts % 10 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("{0:n0}", successCount);
                    Console.ResetColor();
                    Console.Write(" successful connections, ");
                    Console.ForegroundColor = failCount == 0 ? ConsoleColor.White : ConsoleColor.Red;
                    Console.Write("{0:n0}", failCount);
                    Console.ResetColor();
                    Console.WriteLine(" failed connections. Hit Ctrl+C to quit any time.");
                }

                var start = Stopwatch.StartNew();
                var connected = false;
                var sslEstablished = false;

                try
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("Connect: ");
                    Console.ResetColor();

                    var bytesRead = 0;

                    if (dataSize > 0)
                    {
                        var dataToBeSent = 1024 * 1024 * dataSize;

                        while (dataToBeSent > 0)
                        {
                            var data = new string('A', (dataToBeSent < chunkSizeInBytes ? dataToBeSent : chunkSizeInBytes));
                            SendRequest(hostname, port, enableSSL, out bytesRead, out connected, out sslEstablished, data);
                            dataToBeSent -= data.Length;
                        }
                    }
                    else
                    {
                        SendRequest(hostname, port, enableSSL, out bytesRead, out connected, out sslEstablished);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success! {0:n0}ms, {1:n0} bytes read", start.ElapsedMilliseconds, bytesRead);
                    Console.ResetColor();
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed! {0:n0}ms; connected: {1}; SSL: {2}", start.ElapsedMilliseconds, connected, sslEstablished);
                    Console.ResetColor();
                    Console.WriteLine(ex);
                    failCount++;
                }

                Thread.Sleep(500);
            }
        }

        private static void SendRequest(string hostname, int port, bool enableSSL, out int bytesRead, out bool connected, out bool sslEstablished, string data = null)
        {
            using (var client = new TcpClient())
            {
                client.SendTimeout = (int)sendReceiveTimeout.TotalMilliseconds;
                client.ReceiveTimeout = (int)sendReceiveTimeout.TotalMilliseconds;
                client.Connect(hostname, port);

                using (var stream = client.GetStream())
                {
                    connected = true;

                    if (enableSSL)
                    {
                        using (var ssl = new SslStream(stream, false, CertificateValidator, LocalCertificateSelection))
                        {
                            sslEstablished = true;

                            ssl.AuthenticateAsClient(hostname, null, SslProtocols.Tls, false);

                            ExecutePingOnStream(hostname, out bytesRead, data, ssl);
                        }
                    }
                    else
                    {
                        sslEstablished = false;
                        ExecutePingOnStream(hostname, out bytesRead, data, stream);
                    }
                }
            }
        }

        private static void ExecutePingOnStream(string hostname, out int bytesRead, string data, Stream stream)
        {
            var writer = new StreamWriter(stream);
            writer.WriteLine("GET / HTTP/1.1");
            writer.WriteLine("Host: " + hostname);
            writer.WriteLine(data);
            writer.WriteLine();
            writer.Flush();

            using (var reader = new StreamReader(stream))
            {
                bytesRead = reader.ReadToEnd().Length;
            }
        }

        private static X509Certificate LocalCertificateSelection(object sender, string targethost, X509CertificateCollection localcertificates, X509Certificate remotecertificate, string[] acceptableissuers)
        {
            return null;
        }

        private static bool CertificateValidator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        [Verb]
        public static void Pong(
            [DefaultValue(10933)]
            int port,
            [DefaultValue(false),
            Description("Disable SSL")]
            bool noSSL)
        {
            var enableSSL = !noSSL;
            Console.WriteLine("Listening on port " + port + (enableSSL ? " With SSL" : " Without SSL"));
            RunServer(port, enableSSL);
        }

        private static int RunServer(int port, bool enableSSL)
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            listener.Start();

            Accept(enableSSL);

            Console.WriteLine("Listening, hit <Enter> to exit...");
            Console.ReadLine();
            isStopped = true;

            return 0;
        }

        static void Accept(bool enableSSL)
        {
            if (isStopped)
            {
                listener.Stop();
                return;
            }

            listener.BeginAcceptTcpClient(r =>
            {
                try
                {
                    // Just a nicety - race still makes ObjectDisposedException possible
                    if (isStopped)
                        return;

                    TcpClient client;
                    try
                    {
                        client = listener.EndAcceptTcpClient(r);
                    }
                    finally
                    {
                        Accept(enableSSL);
                    }

                    Console.WriteLine("Accepted TCP client " + client.Client.RemoteEndPoint);
                    ExecuteRequest(client, enableSSL);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("TCP client error: " + ex);
                }
            }, null);
        }

        static void ExecuteRequest(TcpClient client, bool enableSSL)
        {
            RemoteCertificateValidationCallback validate = (sender, clientCertificate, chain, sslPolicyErrors) => true;

            try
            {
                using (var stream = client.GetStream())
                {
                    if (enableSSL)
                    {
                        using (var ssl = new SslStream(stream, false, validate))
                        {
                            // We don't actually validate the client, accepting anything; validation
                            // status is reflected in validClientThumprint != null.
                            ssl.AuthenticateAsServer(serverCertificate, true, SslProtocols.Tls, false);

                            ExecutePongOnStream(ssl);
                        }
                    }
                    else
                    {
                        ExecutePongOnStream(stream);
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine("Client failed authentication: " + ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled error when processing request from client: " + ex);
            }
            finally
            {
                client.Close();
            }
        }

        private static void ExecutePongOnStream(Stream ssl)
        {
            var reader = new StreamReader(ssl);

            string line;
            int bytesRead = 0;
            while (!String.IsNullOrEmpty((line = reader.ReadLine())))
            {
                bytesRead += line.Length;
            }
            Console.WriteLine(bytesRead + " bytes read");

            var writer = new StreamWriter(ssl);
            writer.WriteLine("HTTP/1.0 200 OK");
            writer.WriteLine("Content-Length: " + response.Length);
            writer.WriteLine();
            writer.WriteLine(response);
            writer.Flush();
            ssl.Flush();
        }
    }
}