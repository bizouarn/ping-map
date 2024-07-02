using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Ping.Core;

public class PingUtils
{
    private static DateTime _lastLog = DateTime.MinValue;

    private static async Task<bool> Ping(IpV4 ip)
    {
        var pingTimeout = TimeSpan.FromMilliseconds(200);

        if (!ip.IsValid())
            return false;
        if (ip.address[0] == 127)
            return true;

        using var ping = new System.Net.NetworkInformation.Ping();
        byte[] buffer = [1];
        try
        {
            if (_lastLog.AddSeconds(10) < DateTime.Now)
            {
                Console.WriteLine("ping : " + ip);
                _lastLog = DateTime.Now;
            }

            var reply = await ping.SendPingAsync(ip.ToString(), pingTimeout, buffer);
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
        var i = (byte)short.Parse(ipTab[0]);
        var j = (byte)short.Parse(ipTab[1]);

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
        foreach (var k in Constantes.ByteRange)
        {
            if (k % 25 == 0)
                Console.WriteLine("work : " + filePath + " " + k * 100 / 255 + "%");
            foreach (var l in Constantes.ByteRange)
            {
                byteArray[index] = await Ping(new IpV4(i, j, l, k)) ? (byte) 1 : (byte) 0;
                index++;
            }
        }

        await File.WriteAllBytesAsync(filePath, byteArray);

        Console.WriteLine("write : " + filePath);
    }


    private static async Task AppendPingResults(string filePath, byte i, byte j)
    {
        var file = await File.ReadAllBytesAsync(filePath);

        if (file.All(byteValue => byteValue == 1)) return;

        var byteArray = new byte[256 * 256];
        var index = 0;
        foreach (var k in Constantes.ByteRange)
        {
            if (k % 25 == 0)
                Console.WriteLine("work : " + filePath + " " + k * 100 / 255 + "%");
            foreach (var l in Constantes.ByteRange)
            {
                if (file[index] == 0) byteArray[index] = await Ping(new IpV4(i, j, k, l)) ? (byte) 1 : (byte) 0;
                index++;
            }
        }

        await File.WriteAllBytesAsync(filePath, byteArray);
        Console.WriteLine("write : " + filePath);
    }
}