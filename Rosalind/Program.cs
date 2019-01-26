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

        static void Main(string[] args)
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
                        rosa.Request(request).ContinueWith(async response => Console.WriteLine(await response));
                        break;
                }

            } while (pr.GetType() != typeof(Shiolink.Unload));

            return;
        }
    }
}
