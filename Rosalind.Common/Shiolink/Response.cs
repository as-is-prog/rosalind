using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Shiolink
{
    internal enum StatusCode {
        INTERNAL_SERVER_ERROR,
        OK,
        NO_CONTENT,
        BAD_REQUEST
    }

    internal static class StatusCodeUtil
    {
        public static string ToStringFromEnum(this StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.OK:
                    return "200 OK";
                case StatusCode.NO_CONTENT:
                    return "204 No Content";
                case StatusCode.BAD_REQUEST:
                    return "400 Bad Request";
                case StatusCode.INTERNAL_SERVER_ERROR:
                    return "500 Internal Server Error";
                default:
                    throw new ArgumentException("This is not status code.");
            }
        }
    }

    internal class Response
    {
        private static readonly string DEFAULT_RESPONSE_VERSION = "3.0";
        
        private static readonly string SENDER_HEADSTR = "Sender: ";
        private static readonly string CHARSET_HEADSTR = "Charset: ";
        private static readonly string VALUE_HEADSTR = "Value: ";


        public string Version { get; private set; } = DEFAULT_RESPONSE_VERSION;
        public string Charset { get; set; }
        public StatusCode Status { get; set; }
        public string Sender { get; private set; }
        public string Value { get; set; }

        public Response(StatusCode status, string sender = null)
        {
            Status = status;
            Sender = sender;
        }

        public override string ToString()
        {
            var retStr = new StringBuilder(String.Format("SHIORI/{0} {1}\r\n", Version, Status.ToStringFromEnum()));

            if (null != Sender) retStr.AppendFormat("{0}{1}\r\n", SENDER_HEADSTR, Sender);
            if (null != Charset) retStr.AppendFormat("{0}{1}\r\n", CHARSET_HEADSTR, Charset);
            if (null != Value) retStr.AppendFormat("{0}{1}\r\n", VALUE_HEADSTR, Value);
            retStr.AppendLine();
            return retStr.ToString();
        }
    }
}
