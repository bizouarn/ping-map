using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace pingCore
{
    internal class Program
    {
        static async Task Main()
        {
            var start = "0.0.0.1";
            var end = "255.255.255.255";

            var startIP = start.Split('.');
            var endIP = end.Split('.');

            var tasks = new List<Task>();

            for (var i = int.Parse(startIP[0]); i <= int.Parse(endIP[0]); i++)
            for (var j = int.Parse(startIP[1]); j <= int.Parse(endIP[1]); j++)
                tasks.Add(PingRange(i + "." + j));

            await Task.WhenAll(tasks);
        }

        private static async Task<bool> Ping(string ip)
        {
            using var ping = new Ping();
            byte[] buffer = { 1 };
            try
            {
                var reply = await ping.SendPingAsync(ip, 55, buffer); // Effectue un ping avec un délai de 5 secondes

                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        private static async Task PingRange(string ip)
        {
            var ipTab = ip.Split('.');
            var file = $"res/{ip}.txt";
            Directory.Exists("res");
            {
                Directory.CreateDirectory("res");
            }
            Console.WriteLine(file);

            using (var writer = File.CreateText(file))
            {
                var i = int.Parse(ipTab[0]);
                var j = int.Parse(ipTab[1]);

                for (var k = 0; k <= 255; k++)
                {
                    Console.WriteLine($"{i}.{j}.{k}.X");
                    var tasks = new List<Task<bool>>();
                    for (var l = 0; l <= 255; l++)
                    {
                        tasks.Add(Ping($"{i}.{j}.{k}.{l}"));
                    }

                    var results = await Task.WhenAll(tasks);
                    foreach (var result in results) await writer.WriteAsync(result ? "1" : "0");
                    await writer.WriteLineAsync();
                    await writer.FlushAsync();
                }
            }
        }
    }
}
