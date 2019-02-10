﻿#define LOGGING_off
using Shiorose;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    class Shiori
    {
        public static readonly Encoding DEFAULT_CHARSET = Encoding.UTF8;
#if LOGGING
        public static readonly System.IO.TextWriter logFile = System.IO.File.AppendText("error_log.txt");
#endif
        static async Task Main(string[] args)
        {
            // TODO: 設定ファイル(ini)読み込み？

            Console.InputEncoding = DEFAULT_CHARSET;
            Console.OutputEncoding = DEFAULT_CHARSET;

            Shiolink.Load firstLoad = Shiolink.Protocol.Parse(Console.In) as Shiolink.Load;

            Rosalind rosa = await Rosalind.Load(firstLoad);

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
#if LOGGING
                        logFile.WriteLine("\\\\ request\r\n\r\n" + request + "\r\n");
#endif
                        Shiolink.Response res = await rosa.Request(request);
                        Console.WriteLine(res);
#if LOGGING
                        logFile.WriteLine("\\\\ res\r\n\r\n" + res + "\r\n");
#endif
                        break;
                }

            } while (pr.GetType() != typeof(Shiolink.Unload));

            rosa.Unload(pr as Shiolink.Unload);

#if LOGGING
            logFile.Close();
#endif
            return;
        }
    }
}