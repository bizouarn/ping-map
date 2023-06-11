using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pingCore.Ping;

namespace pingCore
{
    internal class Program
    {
        private const int MAX_TASK = 64;
        private const int DELAY = 200;

        private static async Task Main()
        {
            const string start = "0.0";
            const string end = "255.255";

            var startIP = start.Split('.');
            var endIP = end.Split('.');

            for (var i = int.Parse(startIP[0]); i <= int.Parse(endIP[0]); i++)
            for (var j = int.Parse(startIP[1]); j <= int.Parse(endIP[1]); j++)
            {
                await PingUtils.PingRange(i + "." + j);
            }
        }
    }
}