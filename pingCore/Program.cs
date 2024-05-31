using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ping.Core;

internal class Program
{
    private const int MAX_TASK = 5;

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

        var startIP = start.Split('.');
        var endIP = end.Split('.');

        var tasks = new List<Task>();
        var listRange = new List<(int, int)>();
        for (var i = int.Parse(startIP[0]); i <= int.Parse(endIP[0]); i++)
        for (var j = int.Parse(startIP[1]); j <= int.Parse(endIP[1]); j++)
            listRange.Add((i, j));

        var random = new Random();
        while (listRange.Count > 0)
        {
            var indiceAleatoire = random.Next(0, listRange.Count);
            if (indiceAleatoire < listRange.Count)
            {
                var (i, j) = listRange[indiceAleatoire];
                listRange.RemoveAt(indiceAleatoire);
                tasks.Add(PingUtils.PingRange(i + "." + j));
                if (tasks.Count >= MAX_TASK)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
        }

        await Task.WhenAll(tasks);
    }
}