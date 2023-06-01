using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pingCore.Ping;

namespace pingCore
{
    internal class Program
    {
        private const int MAX_TASK = 64;
        private const int DELAY = 1000;

        private static async Task Main()
        {
            var start = "0.0.0.1";
            var end = "255.255.255.255";

            var startIP = start.Split('.');
            var endIP = end.Split('.');

            var tasks = new List<WeakReference>();

            for (var i = int.Parse(startIP[0]); i <= int.Parse(endIP[0]); i++)
            for (var j = int.Parse(startIP[1]); j <= int.Parse(endIP[1]); j++)
            {
                tasks.Add(new WeakReference(PingUtils.PingRange(i + "." + j)));
                while (tasks.Count >= MAX_TASK)
                {
                    DeleteListInactiveRef(ref tasks);
                    await Task.Delay(DELAY * tasks.Count);
                    Console.WriteLine(i / (int.Parse(endIP[0]) > 0 ? int.Parse(endIP[0]) : 1) * 100 + "%" +
                                      $" ({tasks.Count}/{MAX_TASK})");
                }
            }

            while (tasks.Count > 0)
            {
                await Task.Delay(100);
                DeleteListInactiveRef(ref tasks);
            }
        }

        private static void DeleteListInactiveRef(ref List<WeakReference> list)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var weakRef = list[i];
                if (!weakRef.IsAlive) list.RemoveAt(i);

                if (weakRef.Target is Task task && task.IsCompleted) list.RemoveAt(i);
            }
        }
    }
}