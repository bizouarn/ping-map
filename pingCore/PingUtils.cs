using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Ping.Core
{
    public class PingUtils
    {
        private const int PingTimeout = 1000;

        private static async Task<bool> Ping(IpV4 ip)
        {
            if (!ip.IsValid())
                return false;
            if (ip.address[0] == 127)
                return true;

            using var ping = new System.Net.NetworkInformation.Ping();
            byte[] buffer = { 1 };
            try
            {
                var reply = await ping.SendPingAsync(ip.ToString(), PingTimeout, buffer);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static async Task PingRange(string ip)
        {
            var ipTab = ip.Split('.');
            var directoryPath = Constantes.InputDirectory;

            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, $"{ip}.bin");
            var i = int.Parse(ipTab[0]);
            var j = int.Parse(ipTab[1]);

            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 256 * 256)
                {
                    await AppendPingResults(filePath, i, j);
                    return;
                }

                Console.WriteLine($"Delete {filePath}");
                File.Delete(filePath);
            }

            var byteArray = new byte[256 * 256];
            var index = 0;
            var tasks = new List<Task>();
            for (var k = 0; k <= 255; k++)
            {
                if (k % 25 == 0)
                    Console.WriteLine("work : " + filePath + " " + k * 100 / 255 + "%");
                for (var l = 0; l <= 255; l++)
                {
                    var lCopy = l;
                    var kCopy = k;
                    var indexCopy = index;
                    var task = Task.Run(async () =>
                    {
                        byteArray[indexCopy] = await Ping(new IpV4(i, j, kCopy, lCopy)) ? (byte)1 : (byte)0;
                    });
                    tasks.Add(task);
                    index++;
                }
            }

            await Task.WhenAll(tasks);

            await File.WriteAllBytesAsync(filePath, byteArray);

            Console.WriteLine("write : " + filePath);
        }


        private static async Task AppendPingResults(string filePath, int i, int j)
        {
            var file = await File.ReadAllBytesAsync(filePath);

            if (file.All(byteValue => byteValue == 1)) return;

            var byteArray = new byte[256 * 256];
            var index = 0;
            var tasks = new List<Task>();
            for (var k = 0; k <= 255; k++)
            {
                if (k % 25 == 0)
                    Console.WriteLine("work : " + filePath + " " + k * 100 / 255 + "%");
                for (var l = 0; l <= 255; l++)
                {
                    if (file[index] == 0)
                    {
                        var kCopy = k;
                        var lCopy = l;
                        var indexCopy = index;
                        var task = Task.Run(async () =>
                        {
                            byteArray[indexCopy] = await Ping(new IpV4(i, j, kCopy, lCopy)) ? (byte)1 : (byte)0;
                        });
                        tasks.Add(task);
                    }

                    index++;
                }
            }

            await Task.WhenAll(tasks);

            await File.WriteAllBytesAsync(filePath, byteArray);
            Console.WriteLine("write : " + filePath);
        }
    }
}