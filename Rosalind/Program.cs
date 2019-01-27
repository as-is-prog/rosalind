using Shiorose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    class Program
    {
        public static readonly string DEFAULT_CHARSET = "UTF-8";
        public static readonly System.IO.TextWriter logFile = System.IO.File.AppendText("error_log.txt");

        static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Shiolink.Load firstLoad = Shiolink.Protocol.Parse(Console.In) as Shiolink.Load;

            Rosalind rosa = new Rosalind(firstLoad);

            Shiolink.Protocol pr;
            do
            {
                pr = Shiolink.Protocol.Parse(Console.In);
                switch (pr)
                {
                    case Shiolink.Load load:
                        throw new InvalidOperationException("Unexpected second load.");
                    case Shiolink.Sync sync:
                        Console.WriteLine(sync.SyncStr);

                        Shiolink.Request request = Shiolink.Request.Parse(Console.In);
                        await rosa.Request(request).ContinueWith(async response => Console.WriteLine(await response));
                        break;
                }

            } while (pr.GetType() != typeof(Shiolink.Unload));

            logFile.Close();
            return;
        }
    }
}
