using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ping.Core.Utils;

namespace Ping.Core;

internal class Program
{
    private const int _maxTask = 255;

    private static async Task Main(string[] args)
    {
        var start = "0.0";
        var end = "255.255";
        if (args.Length == 1)
        {
            if (args[0].Contains('-'))
            {
                var range = args[0].Split('-');
                start = range[0] + ".0";
                end = range[1] + ".255";
            }
            else
            {
                start = args[0] + ".0";
                end = args[0] + ".255";
            }
        }

        var startIp = start.Split('.');
        var endIp = end.Split('.');

        var tasks = new List<Task>();
        var listRange = new List<(int, int)>();
        for (var i = int.Parse(startIp[0]); i <= int.Parse(endIp[0]); i++)
        for (var j = int.Parse(startIp[1]); j <= int.Parse(endIp[1]); j++)
            listRange.Add((i, j));

        var random = new Random();
        while (listRange.Count > 0)
        {
            var indiceAleatoire = random.Next(0, listRange.Count);
            if (indiceAleatoire < listRange.Count)
            {
                var (i, j) = listRange[indiceAleatoire];
                listRange.RemoveAt(indiceAleatoire);
                
                var directoryPath = Constantes.InputDirectory;
                var ip = i + "." + j;
                var filePath = Path.Combine(directoryPath, $"{ip}.bin");
                if(!File.Exists(filePath))
                    tasks.Add(PingUtils.PingRange(ip));

                if (tasks.Count >= _maxTask)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
        }

        await Task.WhenAll(tasks);
    }
}