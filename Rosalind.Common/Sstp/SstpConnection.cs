using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Shiorose.Sstp
{
    /// <summary>
    /// SSTPを簡単に送るためのクラス
    /// </summary>
    public class SstpConnection
    {
        private string sender;
        private string address;
        private string charset = "UTF-8";
        private int port;

        private const bool DEBUG = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public SstpConnection(string sender, string hostname = "127.0.0.1", int port = 9801)
        {
            this.sender = sender;
            this.port = port;

            address = hostname;
        }
        
        /// <summary>
        /// NOTIFY1.1を送信します
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public string SendNotify1_1(string eventName, params string[] reference)
        {
            string sendDataStr = "NOTIFY SSTP/1.1\r\n" + "Sender: " + sender + "\r\n" + "Event: " + eventName + "\r\n";

            StringBuilder sb = new StringBuilder(sendDataStr);

            for(var i = 0; i < reference.Length; i++)
            {
                sb.Append("Reference");
                sb.Append(i);
                sb.Append(": ");
                sb.Append(reference[i]);
                sb.Append("\r\n");
            }

            sb.Append("Charset: " + charset + "\r\n\r\n");

            string response;
            try
            {
                response = Send(sb.ToString());
            }
            catch (IOException)
            {
                response = "IOException";
            }

            return response;
        }

        /// <summary>
        /// 指定のSSTPProtocolを送信します
        /// </summary>
        /// <param name="sstp"></param>
        /// <returns></returns>
        internal string Send(SstpProtocol sstp)
        {
            Send(sstp.ToSstpString());

            return "";
        }

        /// <summary>
        /// SSTPのポートに任意の文字列を送信します。
        /// </summary>
        /// <param name="sendDataStr"></param>
        /// <returns></returns>
        private string Send(string sendDataStr)
        {
            try
            {
                Encoding encode = Encoding.GetEncoding(charset);
            
                var tcp = new TcpClient(address, port);
                var sendBytes = encode.GetBytes(sendDataStr);

                var ns = tcp.GetStream();
                ns.Write(sendBytes, 0, sendBytes.Length);
                ns.Flush();

                System.Diagnostics.Debug.WriteLine(string.Join("", encode.GetChars(sendBytes)));

                var sr = new StreamReader(ns);

                var response = sr.ReadLine();

                tcp.Close();

                return "";//response;
            }
            catch
            {
                return "error";
            }

        }
    }
}
