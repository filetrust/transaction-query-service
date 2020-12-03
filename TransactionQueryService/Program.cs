using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Glasswall.Administration.K8.TransactionQueryService
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.ConfigureHttpsDefaults(opts =>
                        {
                            opts.ServerCertificate = LoadCertificate("/etc/ssl/certs/server.crt", "/etc/ssl/private/server.key");
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });

        private static X509Certificate2 LoadCertificate(string crtPath, string keyPath)
        {
            Console.WriteLine("Loading Self-Signed Certificate");

            const string dashes = "-----";
            var keyPem = File.ReadAllText(keyPath);
            var index0 = keyPem.IndexOf(dashes, StringComparison.Ordinal);
            var index1 = keyPem.IndexOf('\n', index0 + dashes.Length);
            var index2 = keyPem.IndexOf(dashes, index1 + 1, StringComparison.Ordinal);
            var keyDer = Convert.FromBase64String(keyPem.Substring(index1, index2 - index1));
            X509Certificate2 certWithKey;

            using (var certOnly = new X509Certificate2(crtPath))
            using (var rsa = RSA.Create())
            {
                // For "BEGIN PRIVATE KEY"
                rsa.ImportPkcs8PrivateKey(keyDer, out _);
                certWithKey = certOnly.CopyWithPrivateKey(rsa);
            }

            Console.WriteLine("Using cert: " + certWithKey);
            return certWithKey;
        }
    }
}



