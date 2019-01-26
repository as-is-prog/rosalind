﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Shiolink
{
    public enum RequestMethod
    {
        GET,
        NOTIFY,
        NONE
    };

    public static class RequestMethodUtil
    {
        public static string ToStringFromEnum(this RequestMethod requestMethod)
        {
            switch (requestMethod)
            {
                case RequestMethod.GET:
                    return "GET";
                case RequestMethod.NOTIFY:
                    return "NOTIFY";
                default:
                    return "NONE";
            }
        }

        public static RequestMethod StringToRequestMethod(string methodStr)
        {
            switch (methodStr)
            {
                case "GET":
                    return RequestMethod.GET;
                case "NOTIFY":
                    return RequestMethod.NOTIFY;
                default:
                    throw new FormatException(methodStr + " is not SHIORI request method.");
            }
        }
    }

    public class Request : Protocol
    {

        private static readonly string SENDER_HEADSTR = "Sender: ";
        private static readonly string CHARSET_HEADSTR = "Charset: ";
        private static readonly string SECURITYLEVEL_HEADSTR = "SecurityLevel: ";
        private static readonly string STATUS_HEADSTR = "Status: ";
        private static readonly string ID_HEADSTR = "ID: ";
        private static readonly string REFERENCE_HEADSTR = "Reference";


        public RequestMethod Method { get; private set; }
        public string Version { get; private set; }
        public string Charset { get; private set; }
        public string Sender { get; private set; }
        public string SecurityLevel { get; private set; }
        public string Status { get; set; }
        public string ID { get; private set; }
        public string[] References { get; private set; }

        public static new Request Parse(System.IO.TextReader stdIn)
        {
            var request = new Request();

            var tmp_reference = new List<string>();

            var s = stdIn.ReadLine();

            try {
                request.Method = RequestMethodUtil.StringToRequestMethod(s.Substring(0, s.IndexOf(' ')));
                request.Version = s.Substring(s.LastIndexOf('/') + 1, 3);
            }
            catch
            {
                throw new FormatException("SHIORI request parse error. [" + s + "]");
            }

            while((s = stdIn.ReadLine()) != "")
            {
                
                if (s.StartsWith(SENDER_HEADSTR))
                {
                    request.Sender = s.Substring(SENDER_HEADSTR.Length);
                }
                else if (s.StartsWith(CHARSET_HEADSTR))
                {
                    request.Charset = s.Substring(CHARSET_HEADSTR.Length);
                }
                else if (s.StartsWith(SECURITYLEVEL_HEADSTR))
                {
                    request.SecurityLevel = s.Substring(SECURITYLEVEL_HEADSTR.Length);
                }
                else if (s.StartsWith(STATUS_HEADSTR))
                {
                    request.Status = s.Substring(STATUS_HEADSTR.Length);
                }
                else if (s.StartsWith(ID_HEADSTR))
                {
                    request.ID = s.Substring(ID_HEADSTR.Length);
                }
                else if (s.StartsWith(REFERENCE_HEADSTR))
                {
                    tmp_reference.Add(s.Substring(s.IndexOf(": ") + 2));
                }
                else
                {
                    throw new FormatException("Parse error. str: " + s);
                }
            }

            request.References = tmp_reference.ToArray();

            return request;
        }

        public override string ToString()
        {
            var reqHeadStr = Method.ToStringFromEnum() + (Version == "2.6" ? " Version" : ""); 
            var retStr = new StringBuilder(String.Format("{0} SHIORI/{1}\r\n", reqHeadStr, Version));
            
            if (null != Charset) retStr.AppendFormat("{0}{1}\r\n", CHARSET_HEADSTR, Charset.ToString());
            if (null != Sender) retStr.AppendFormat("{0}{1}\r\n", SENDER_HEADSTR, Sender);
            if (null != SecurityLevel) retStr.AppendFormat("{0}{1}\r\n", SECURITYLEVEL_HEADSTR, SecurityLevel);
            if (null != ID) retStr.AppendFormat("{0}{1}\r\n", ID_HEADSTR, ID);
            var rIdx = 0;
            References.ToList().ForEach(r => retStr.AppendFormat("{0}{1}: {2}\r\n", REFERENCE_HEADSTR, rIdx++, r));
            retStr.AppendLine();
            return retStr.ToString();
        }
    }
}
