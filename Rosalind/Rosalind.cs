using Shiorose.Shiolink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    public class Rosalind
    {
        public static readonly string NAME = "Rosalind";
        private static readonly string SHIORI_PROTOCOL_VERSION = "3.0";

        private readonly string shioriDir;

        public Rosalind(Load load)
        {
            shioriDir = load.ShioriDir;
        }

        public async Task<Response> Request(Shiolink.Request req)
        {
            return await Task.Run(() =>
            {
                if (req.Version != SHIORI_PROTOCOL_VERSION)
                {
                    return CreateBadRequestResponse();
                }

                switch (req.Method)
                {
                    case RequestMethod.GET:
                        var retValue = "testdata";
                        switch (req.ID)
                        {
                            case "version":
                                retValue = "0.0.0";
                                break;
                            case "name":
                                retValue = "Rosalind";
                                break;
                            case "craftman":
                                retValue = "as-is-prog";
                                break;
                            case "craftmanw":
                                retValue = "AS-IS";
                                break;
                            case "OnBoot":
                            case "OnFirstBoot":
                            case "OnGhostChanged":
                                retValue = @"\u\s[-1]\h\s[0]こんちは";
                                break;
                            case "OnClose":
                                retValue = @"\u\s[-1]\h\s[0]さいなら";
                                break;
                            case "homeurl":
                            case "username":
                            case "sakura.recommendsites":
                            case "sakura.portalsites":
                            case "kero.recommendsites":
                            case "kero.portalsites":
                                return CreateNoContentResponse();
                            default:
                                return CreateNoContentResponse();
                        }
                        return CreateOKResponse(retValue);
                    case RequestMethod.NOTIFY:
                        return CreateOKResponse(null);
                }

                return CreateBadRequestResponse();
            });

        }

        public void Unload(Shiolink.Unload unload)
        {

        }

        public static Response CreateOKResponse(string value)
        {
            var res = new Response(StatusCode.OK, NAME);

            res.Charset = Program.DEFAULT_CHARSET;

            res.Value = value;

            return res;
        }

        public static Response CreateNoContentResponse()
        {
            var res = new Response(StatusCode.NO_CONTENT);

            res.Charset = Program.DEFAULT_CHARSET;

            return res;
        }

        public static Response CreateBadRequestResponse()
        {
            var res = new Response(StatusCode.BAD_REQUEST);

            return res;
        }

    }
}
