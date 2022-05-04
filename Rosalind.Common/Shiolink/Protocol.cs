using System;
using System.IO;

namespace Shiorose.Shiolink
{
    internal abstract class Protocol
    {
        public static Protocol Parse(EncodingTextReader stdIn)
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
