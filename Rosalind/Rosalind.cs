using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Shiorose.Shiolink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    public class Rosalind
    {
        public static readonly string NAME = "Rosalind";
        public static readonly string VERSION = "0.0.0";
        private static readonly string CRAFTMAN = "as-is-prog";
        private static readonly string CRAFTMAN_W = "AS-IS";

        private static readonly string SHIORI_PROTOCOL_VERSION = "3.0";

        public readonly string shioriDir;
        private Ghost ghost;

        private Rosalind(Load load)
        {
            shioriDir = load.ShioriDir;
        }

        public async static Task<Rosalind> Load(Load load)
        {
            Rosalind rosa = new Rosalind(load);

            var files = Directory.GetFiles(rosa.shioriDir, "*.csx", SearchOption.AllDirectories).Where(f => !(f.EndsWith("Ghost.csx")));

            var imp = String.Join("\r\n", files.Select(f => string.Format("#load \"{0}\"", f))) + "\r\n";

            var script = CSharpScript.Create<Ghost>(imp + File.ReadAllText(rosa.shioriDir + "Ghost.csx"))
                                     .WithOptions(ScriptOptions.Default.WithReferences(Assembly.GetEntryAssembly()));
            
            rosa.ghost = (await script.RunAsync()).ReturnValue;

            return rosa;
        }

        public async Task<Response> Request(Request req)
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
                        return CreateResponseOfGETRequest(req);
                    case RequestMethod.NOTIFY:
                        return CreateOKResponse(null);
                }

                return CreateBadRequestResponse();
            });

        }

        private Response CreateResponseOfGETRequest(Request req)
        {
            var retValue = "";

            switch (req.ID)
            {
                /* SHIORI property */
                case "version":
                    retValue = VERSION;
                    break;
                case "name":
                    retValue = NAME;
                    break;
                case "craftman":
                    retValue = CRAFTMAN;
                    break;
                case "craftmanw":
                    retValue = CRAFTMAN_W;
                    break;
                /* Ghost property */
                case "homeurl":
                    retValue = ghost.Homeurl;
                    break;
                case "username":
                case "sakura.recommendsites":
                case "sakura.portalsites":
                case "kero.recommendsites":
                case "kero.portalsites":
                    retValue = "";
                    break;
                /* SHIORI event (supposes) */
                case "OnBoot":
                case "OnFirstBoot":
                case "OnGhostChanged":
                    retValue = ghost.OnBoot();
                    break;
                case "OnClose":
                    retValue = ghost.OnClose();
                    break;
                /* SHIORI event (other) */
                default:
                    try
                    {
                        retValue = ghost.GetType().GetMethod(req.ID).Invoke(ghost, null) as string;
                    }
                    catch { }
                    break;
            }

            if (retValue != "")
            {
                return CreateOKResponse(retValue);
            }
            else
            {
                return CreateNoContentResponse();
            }
        }

        public void Unload(Shiolink.Unload unload)
        {

        }

        public static Response CreateOKResponse(string value)
        {
            var res = new Response(StatusCode.OK, NAME);

            res.Charset = "UTF-8";

            res.Value = value;

            return res;
        }

        public static Response CreateNoContentResponse()
        {
            var res = new Response(StatusCode.NO_CONTENT);

            res.Charset = "UTF-8";

            return res;
        }

        public static Response CreateBadRequestResponse()
        {
            var res = new Response(StatusCode.BAD_REQUEST);

            return res;
        }

    }
}
