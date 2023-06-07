using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace pingCore.Ping
{
    public class PingUtils
    {
        private const int PingTiemout = 1000;

        private static async Task<bool> Ping(string ip)
        {
            using var ping = new System.Net.NetworkInformation.Ping();
            byte[] buffer = {1};
            try
            {
                var reply = await ping.SendPingAsync(ip, PingTiemout,
                    buffer); // Effectue un ping avec un délai de 5 secondes
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
            var directoryPath = Constantes.InputDirectory;

            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, $"{ip}.txt");
            if (File.Exists(filePath)) return;

            await using var writer = new StreamWriter(filePath);

            var i = int.Parse(ipTab[0]);
            var j = int.Parse(ipTab[1]);

            var tasks = new List<Task<bool>>();

            for (var k = 0; k <= 255; k++)
            {
                for (var l = 0; l <= 255; l++) tasks.Add(Ping($"{i}.{j}.{k}.{l}"));

                var results = await Task.WhenAll(tasks);

                foreach (var result in results) await writer.WriteAsync(result ? "1" : "0");

                await writer.WriteLineAsync();
                tasks.Clear();
            }

            await writer.FlushAsync();
        }
    }
}