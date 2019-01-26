using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Shiolink
{
    public class Sync : Protocol
    {
        public string SyncStr { get; private set; }

        public Sync (string syncStr)
        {
            SyncStr = syncStr;
        }

        public static new Sync Parse(System.IO.TextReader stdIn)
        {
            return Protocol.Parse(stdIn) as Sync;
        }

        public override string ToString()
        {
            return SyncStr + "\r\n";
        }
    }
}
