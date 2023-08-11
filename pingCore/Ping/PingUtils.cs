using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace pingCore.Ping
{
    public class PingUtils
    {
        private const int PingTimeout = 1000;

        private static async Task<bool> Ping(string ip)
        {
            using var ping = new System.Net.NetworkInformation.Ping();
            byte[] buffer = {1};
            try
            {
                try
                {
                    var reply = await ping.SendPingAsync(ip, PingTimeout,
                        buffer); // Effectue un ping avec un délai de 5 secondes
                    return reply.Status == IPStatus.Success;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
            catch (PingException)
            {
                return false;
            }
        }

        public static async Task PingRange(string ip)
        {
            var ipTab = ip.Split('.');
            var directoryPath = Constantes.InputDirectory;

            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, $"{ip}.txt");
            var i = int.Parse(ipTab[0]);
            var j = int.Parse(ipTab[1]);

            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 66048)
                {
                    await AppendPingResults(filePath, i, j);
                    return;
                }

                Console.WriteLine($"Delete {filePath}");
                File.Delete(filePath);
            }

            await using var writer = new StreamWriter(filePath);

            var tasks = new List<Task<bool>>();

            for (var k = 0; k <= 255; k++)
            {
                for (var l = 0; l <= 255; l++)
                {
                    Console.WriteLine($"\t ping {i}.{j}.{k}.{l}");
                    tasks.Add(Ping($"{i}.{j}.{k}.{l}"));
                }

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                    await writer.WriteAsync(result ? "1" : "0");

                Console.WriteLine("write : " + filePath);
                await writer.WriteLineAsync();
            }

            await writer.FlushAsync();
        }


        private static async Task AppendPingResults(string filePath, int i, int j)
        {
            var lines = await File.ReadAllLinesAsync(filePath);

            if (!lines.Any(line => line.Contains("0"))) return;

            for (var k = 0; k < lines.Length; k++)
            {
                var line = lines[k].ToCharArray();
                for (var l = 0; l <= 255; l++)
                    if (line[l] == '0')
                    {
                        Console.WriteLine($"\t ping {i}.{j}.{k}.{l}");
                        line[l] = await Ping($"{i}.{j}.{k}.{l}") ? '1' : '0';
                    }

                lines[k] = new string(line);
            }

            await using var writer = new StreamWriter(filePath);

            foreach (var line in lines) await writer.WriteLineAsync(line);

            await writer.FlushAsync();
        }
    }
}