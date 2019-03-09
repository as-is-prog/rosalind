using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Shiorose.Sstp
{
    class Notify : SstpProtocol
    {
        public string Sender { get; set; } 
        public string Event { get; set; }
        public string Charset { get; set; }
        public IDictionary<int, string> Reference { get; set; }
        

        public override string ToSstpString()
        {
            string sendDataStr = "NOTIFY SSTP/1.1\r\n" + "Sender: " + Sender + "\r\n" + "Event: " + Event + "\r\n";

            StringBuilder sb = new StringBuilder(sendDataStr);

            sb.Append(Reference.Keys.Select((k) => string.Format("Reference{0}: {1}\r\n", k, Reference[k])).Aggregate((a, b) => a + b));

            sb.Append("Charset: " + Charset + "\r\n\r\n");

            return sb.ToString();
        }
    }
}
