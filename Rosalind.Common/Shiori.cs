#define LOGGING_off
using System;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    public class Shiori
    {
        public static readonly Encoding DEFAULT_CHARSET = Encoding.UTF8;
#if LOGGING
        public static readonly System.IO.TextWriter logFile = System.IO.File.AppendText("error_log.txt");
#endif
        internal static async Task Main(string[] args, Func<Shiolink.Load, Task<Rosalind>> loadFunc)
        {
            // TODO: 設定ファイル(ini)読み込み？
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var firstEncoding = Console.InputEncoding;


            var inStream = Console.OpenStandardInput();

            var inTextReader = new EncodingTextReader(inStream)
            {
                Encoding = DEFAULT_CHARSET
            };

            Shiolink.Load firstLoad = Shiolink.Protocol.Parse(inTextReader) as Shiolink.Load;

            inTextReader.Encoding = DEFAULT_CHARSET;
            Console.OutputEncoding = DEFAULT_CHARSET;

            Rosalind rosa = await loadFunc(firstLoad);

            Shiolink.Protocol pr;
            do
            {
                pr = Shiolink.Protocol.Parse(inTextReader);
                switch (pr)
                {
                    case Shiolink.Load load:
                        throw new InvalidOperationException("Unexpected second load.");
                    case Shiolink.Sync sync:
                        Console.WriteLine(sync.SyncStr);
                        Shiolink.Request request = Shiolink.Request.Parse(inTextReader);
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
            inTextReader.Dispose();

#if LOGGING
            logFile.Close();
#endif
            return;
        }
    }
}
