using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Shiolink
{
    internal abstract class Protocol
    {
        public static Protocol Parse(System.IO.TextReader stdIn)
        {
            var s = stdIn.ReadLine();

            if (s.StartsWith("*L:"))
            {
                return new Load(s.Substring(s.IndexOf(':') + 1));
            }
            else if (s.StartsWith("*S:"))
            {
                return new Sync(s);
            }
            else if (s.StartsWith("*U:"))
            {
                return new Unload();
            }
            else
            {
                throw new FormatException("SHIOLINK command parse error. str: " + s);
            }
        }
    }
}
