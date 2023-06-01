using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace pingCore.Ping
{
    public class PingUtils
    {
        private static async Task<bool> Ping(string ip)
        {
            using var ping = new System.Net.NetworkInformation.Ping();
            byte[] buffer = {1};
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

        public static async Task PingRange(string ip)
        {
            var ipTab = ip.Split('.');
            Directory.Exists(Constantes.InputDirectory);
            {
                Directory.CreateDirectory(Constantes.InputDirectory);
            }

            var file = Constantes.InputDirectory + $"\\{ip}.txt";
            if(File.Exists(file))
                return;

            using (var writer = File.CreateText(file))
            {
                var i = int.Parse(ipTab[0]);
                var j = int.Parse(ipTab[1]);

                for (var k = 0; k <= 255; k++)
                {
                    var tasks = new List<Task<bool>>();
                    for (var l = 0; l <= 255; l++) tasks.Add(Ping($"{i}.{j}.{k}.{l}"));

                    var results = await Task.WhenAll(tasks);
                    foreach (var result in results) await writer.WriteAsync(result ? "1" : "0");
                    await writer.WriteLineAsync();
                    await writer.FlushAsync();
                }
            }
        }
    }
}