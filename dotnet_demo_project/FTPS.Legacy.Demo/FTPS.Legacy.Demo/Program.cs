using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;

class Program
{
    static void Main()
    {
        try
        {
            using var tcp = new TcpClient();
            Console.WriteLine("Connecting TCP...");
            tcp.Connect("localhost", 2121); 
            Console.WriteLine("TCP connected.");

            using var netStream = tcp.GetStream();
            using var reader = new StreamReader(netStream, Encoding.ASCII, false, 1024, leaveOpen: true);
            using var writer = new StreamWriter(netStream, Encoding.ASCII, 1024, leaveOpen: true) { NewLine = "\r\n", AutoFlush = true };

            var banner = reader.ReadLine();
            Console.WriteLine("SERVER: " + banner);

            Console.WriteLine("CLIENT: AUTH TLS");
            writer.WriteLine("AUTH TLS");

            var authResponse = reader.ReadLine();
            Console.WriteLine("SERVER: " + authResponse);

            if (authResponse == null || !authResponse.StartsWith("234"))
            {
                Console.WriteLine("AUTH TLS not accepted by server, aborting.");
                return;
            }

            Console.WriteLine("Starting TLS handshake via SslStream...");
            using var ssl = new SslStream(
                netStream,
                leaveInnerStreamOpen: false,
                (sender, certificate, chain, errors) =>
                {
                    // Log what .NET sees
                    Console.WriteLine($"Cert subject: {certificate?.Subject}");
                    Console.WriteLine($"Errors: {errors}");
                    return true; 
                });

            var options = new SslClientAuthenticationOptions {
            TargetHost = "localhost",
            EnabledSslProtocols = SslProtocols.Tls12,

            // Uncomment below to fix the sslv3 alert handshake failure error

            // CipherSuitesPolicy = new CipherSuitesPolicy(new[]
            // {
            //     TlsCipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384
            // })
        };

        ssl.AuthenticateAsClient(options);
            Console.WriteLine("TLS handshake SUCCESS via SslStream.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXCEPTION:");
            Console.WriteLine(ex);
        }
    }
}
