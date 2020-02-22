using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shiorose.Sstp;

namespace Shiorose.Web
{
    /// <summary>
    /// httpとsstpの仲介をするサーバです
    /// </summary>
    public class HttpToSstpServer
    {
        private readonly HttpListener httpListener = new HttpListener();

        /// <summary>
        /// コンストラクタ。この時点ではサーバは起動しません。Start()を呼び出しましょう。
        /// </summary>
        public HttpToSstpServer()
        {
            httpListener.Prefixes.Add("http://127.0.0.1:9881/");
        }

        /// <summary>
        /// サーバをスタートします。
        /// </summary>
        public void Start()
        {
            httpListener.Start();

            Task.Run(() =>
            {
                for (; ; )
                {
                    var c = httpListener.GetContext();
                    OnRequested(c);
                }
            });

        }

        /// <summary>
        /// サーバにリクエストがあった時の処理を行うメソッドです。
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnRequested(HttpListenerContext context)
        {
            object responseValue = null;

            if (context.Request.RawUrl.StartsWith("/api/v1/sstp/notify"))
            {
                Notify content = null;
                if (context.Request.HasEntityBody)
                {
                    using (var sr = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        content = JsonConvert.DeserializeObject<Notify>(sr.ReadToEnd());
                    }
                }

                new SstpConnection(content.Sender).Send(content);

                responseValue = "sstp_sended";
            }
            else
            {
                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        responseValue = OnGETRequested(context);
                        break;
                    case "POST":
                        responseValue = OnPostRequested(context);
                        break;
                    default:
                        break;
                }
            }

            if (responseValue != null)
            {
                var responseJsonStr = JsonConvert.SerializeObject(responseValue);

                HttpListenerResponse res = null;
                StreamWriter sw = null;
                try
                {
                    res = context.Response;
                    res.ContentType = "application/json";
                    sw = new StreamWriter(res.OutputStream);
                    sw.WriteLine(responseJsonStr);
                }
                finally
                {
                    sw.Close();
                    res.Close();
                }
            }
            else
            {
                context.Response.StatusCode = 204;
                context.Response.Close();
            }
        }


        /// <summary>
        /// サーバにGETリクエストがあった時に実行されるメソッドです。
        /// </summary>
        /// <param name="context"></param>
        /// <returns>jsonで返したいオブジェクト。返す値がない場合はnullを返してください。</returns>
        protected virtual object OnGETRequested(HttpListenerContext context)
        {
            return null;
        }

        /// <summary>
        /// サーバにGETリクエストがあった時に実行されるメソッドです。
        /// </summary>
        /// <param name="context"></param>
        /// <returns>jsonで返したいオブジェクト。返す値がない場合はnullを返してください。</returns>
        protected virtual object OnPostRequested(HttpListenerContext context)
        {
            if (context.Request.HasEntityBody)
            {
                using (var sr = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    Console.WriteLine(sr.ReadToEnd());
                }
            }

            return null;
        }

        /// <summary>
        /// サーバを一時停止します。
        /// </summary>
        public void Stop()
        {
            httpListener.Stop();
        }

        /// <summary>
        /// サーバを終了します。
        /// </summary>
        public void Close()
        {
            httpListener.Close();
        }

    }
}
