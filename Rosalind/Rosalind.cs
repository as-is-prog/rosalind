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


            // Script Options
            var ssr = ScriptSourceResolver.Default.WithBaseDirectory(Environment.CurrentDirectory);
            var smr = ScriptMetadataResolver.Default.WithBaseDirectory(Environment.CurrentDirectory);
            var so = ScriptOptions.Default.WithSourceResolver(ssr).WithMetadataResolver(smr);

            var script = CSharpScript.Create<Ghost>(imp + File.ReadAllText(rosa.shioriDir + "Ghost.csx"), so);
            
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
                case "OnFirstBoot":
                    {
                        req.References.TryGetValue(0, out string r0);
                        int vanishCount;
                        Int32.TryParse(r0, out vanishCount);
                        retValue = ghost.OnFirstBoot(req.References, vanishCount);
                    }
                    break;
                case "OnBoot":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(6, out string r6);
                        req.References.TryGetValue(7, out string r7);
                        bool isHalt = r6 == "halt";
                        retValue = ghost.OnBoot(req.References, r0, isHalt, r7);
                    }
                    break;
                case "OnClose":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnClose(req.References, r0);
                    }
                    break;
                case "OnCloseAll":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnCloseAll(req.References, r0);
                    }
                    break;
                case "OnCloseChanged":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(7, out string r7);
                        retValue = ghost.OnGhostChanged(req.References, r0, r1, r2, r3, r7);
                    }
                    break;
                case "OnGhostChanging":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        retValue = ghost.OnGhostChanging(req.References, r0, r1, r2, r3);
                    }
                    break;
                case "OnGhostCalled":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(7, out string r7);
                        retValue = ghost.OnGhostCalled(req.References, r0, r1, r2, r3, r7);
                    }
                    break;
                case "OnGhostCalling":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        retValue = ghost.OnGhostCalling(req.References, r0, r1, r2, r3);
                    }
                    break;
                case "OnGhostCallComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(7, out string r7);
                        retValue = ghost.OnGhostCallComplete(req.References, r0, r1, r2, r7);
                    }
                    break;
                case "OnOtherGhostBooted":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(7, out string r7);
                        retValue = ghost.OnOtherGhostBooted(req.References, r0, r1, r2, r7);
                    }
                    break;
                case "OnOtherGhostChanged":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(14, out string r14);
                        req.References.TryGetValue(15, out string r15);
                        retValue = ghost.OnOtherGhostChanged(req.References, r0, r1, r2, r3, r4, r5, r14, r15);
                    }
                    break;
                case "OnOtherGhostClosed":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(7, out string r7);
                        retValue = ghost.OnOtherGhostClosed(req.References, r0, r1, r2, r7);
                    }
                    break;
                /* SHIORI event (other) */
                default:
                    try
                    {
                        object[] parameters = { req.References };
                        retValue = ghost.GetType().GetMethod(req.ID).Invoke(ghost, parameters) as string;
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
            var res = new Response(StatusCode.OK, NAME)
            {
                Charset = "UTF-8",

                Value = value
            };

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
