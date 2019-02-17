using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Shiorose.Resource;
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
    /// <summary>
    /// SHIORIイベントを解釈する、Rosalindのメインクラス
    /// </summary>
    public class Rosalind
    {
        private static readonly string NAME = "Rosalind";
        private static readonly string VERSION = "0.0.0";
        private static readonly string CRAFTMAN = "as-is-prog";
        private static readonly string CRAFTMAN_W = "AS-IS";

        private static readonly string SHIORI_PROTOCOL_VERSION = "3.0";

        /// <summary>
        /// SHIORIの存在しているディレクトリ
        /// </summary>
        public static string ShioriDir { get; private set; }
        private Ghost ghost;

        private Rosalind(Load load)
        {
            ShioriDir = load.ShioriDir;
        }

        internal async static Task<Rosalind> Load(Load load)
        {
            Rosalind rosa = new Rosalind(load);

            var files = Directory.GetFiles(ShioriDir, "*.csx", SearchOption.AllDirectories).Where(f => !(f.EndsWith("Ghost.csx") || f.EndsWith("SaveData.csx")));

            var imp = String.Join("\r\n", files.Select(f => string.Format("#load \"{0}\"", f))) + "\r\n";

            // Script Options
            var ssr = ScriptSourceResolver.Default.WithBaseDirectory(ShioriDir);
            var smr = ScriptMetadataResolver.Default.WithBaseDirectory(ShioriDir);
            var so = ScriptOptions.Default.WithSourceResolver(ssr).WithMetadataResolver(smr);

            var ghostScript = CSharpScript.Create<Ghost>(imp + File.ReadAllText(ShioriDir + "Ghost.csx"), so);

            rosa.ghost = (await ghostScript.RunAsync()).ReturnValue;
            return rosa;
        }

        internal async Task<Response> Request(Request req)
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
                    retValue = "";
                    break;
                case "sakura.recommendsites":
                    retValue = ghost.SakuraRecommendSites.ToStringFromSites();
                    break;
                case "sakura.portalsites":
                    retValue = ghost.SakuraPortalSites.ToStringFromSites();
                    break;
                case "kero.recommendsites":
                    retValue = ghost.KeroRecommendSites.ToStringFromSites();
                    break;
                case "kero.portalsites":
                    retValue = ghost.KeroPortalSites.ToStringFromSites();
                    break;
                /* SHIORI event (supposes) */
                #region 起動・終了・切り替えイベント
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
                case "OnGhostChanged":
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
                case "OnShellChanged":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);

                        retValue = ghost.OnShellChanged(req.References, r0, r1, r2);
                    }
                    break;
                case "OnShellChanging":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);

                        retValue = ghost.OnShellChanging(req.References, r0, r1, r2);
                    }
                    break;
                case "OnDressupChanged":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        bool isEnable = r2 == "1";
                        req.References.TryGetValue(3, out string r3);

                        retValue = ghost.OnDressupChanged(req.References, r0, r1, isEnable, r3);
                    }
                    break;
                case "OnBalloonChange":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnBalloonChange(req.References, r0, r1);
                    }
                    break;
                case "OnWindowStateRestore":
                    retValue = ghost.OnWindowStateRestore();
                    break;
                case "OnWindowStateMinimize":
                    retValue = ghost.OnWindowStateMinimize();
                    break;
                case "OnFullScreenAppMinimize":
                    retValue = ghost.OnFullScreenAppMinimize();
                    break;
                case "OnFullScreenAppRestore":
                    retValue = ghost.OnFullScreenAppRestore();
                    break;
                case "OnVirtualDesktopChanged":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnVirtualDesktopChanged(req.References, r0, r1);
                    }
                    break;
                case "OnCacheSuspend":
                    retValue = ghost.OnCacheSuspend();
                    break;
                case "OnCacheRestore":
                    retValue = ghost.OnCacheRestore();
                    break;
                // TODO: OnInitialize [NOTIFY]
                // TODO: OnDestroy [NOTIFY]
                case "OnSysResume":
                    {
                        req.References.TryGetValue(0, out string r0);

                        retValue = ghost.OnSysResume(req.References, r0);
                    }
                    break;
                // TODO: OnSysSuspend [NOTIFY]
                case "OnBasewareUpdating":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnBasewareUpdating(req.References, r0, r1);
                    }
                    break;
                case "OnBasewareUpdated":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnBasewareUpdated(req.References, r0, r1);
                    }
                    break;
                #endregion
                #region 入力ボックスイベント
                case "OnTeachStart":
                    retValue = ghost.OnTeachStart();
                    break;
                case "OnTeachInputCancel":
                    {
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnTeachInputCancel(req.References, r1);
                    }
                    break;
                case "OnTeach":
                    retValue = ghost.OnTeach(req.References, req.References.Select(p => p.Value));
                    break;
                case "OnCommunicate":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnCommunicate(req.References, r0, r1, req.References.Skip(2).Select(p => p.Value));
                    }
                    break;
                case "OnCommunicateInputCancel":
                    {
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnCommunicateInputCancel(req.References, r1);
                    }
                    break;
                case "OnUserInput":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnUserInput(req.References, r0, r1);
                    }
                    break;
                case "OnUserInputCancel":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnUserInputCancel(req.References, r0, r1);
                    }
                    break;
                case "inputbox.autocomplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.InputboxAutocomplete(req.References, r0, r1);
                    }
                    break;
                #endregion
                #region ダイアログボックスイベント
                case "OnSystemDialog":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);

                        retValue = ghost.OnSystemDialog(req.References, r0, r1, r2);
                    }
                    break;
                case "OnSystemDialogCancel":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        
                        retValue = ghost.OnSystemDialogCancel(req.References, r0, r1);
                    }
                    break;
                case "OnConfigurationDialogHelp":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);

                        retValue = ghost.OnConfigurationDialogHelp(req.References, r0, r1, r2, r3);
                    }
                    break;
                #endregion
                #region 時間イベント

                case "OnSecondChange":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);

                        retValue = ghost.OnSecondChange(req.References, r0, r1 != "0", r2 != "0", r3 != "0", r4);
                    }
                    break;
                case "OnMinuteChange":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);

                        retValue = ghost.OnMinuteChange(req.References, r0, r1 != "0", r2 != "0", r3 != "0", r4);
                    }
                    break;
                case "OnHourTimeSignal":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);

                        retValue = ghost.OnHourTimeSignal(req.References, r0, r1 != "0", r2 != "0", r3 != "0", r4);
                    }
                    break;

                #endregion
                #region 消滅イベント

                case "OnVanishSelecting":
                    retValue = ghost.OnVanishSelecting();
                    break;
                case "OnVanishSelected":
                    retValue = ghost.OnVanishSelected();
                    break;
                case "OnVanishCancel":
                    retValue = ghost.OnVanishCancel();
                    break;
                case "OnVanishButtonHold":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);

                        retValue = ghost.OnVanishButtonHold(req.References, r0, r1, r2);
                    }
                    break;
                case "OnVanished":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(7, out string r7);

                        retValue = ghost.OnVanished(req.References, r0, r1, r2, r7);
                    }
                    break;
                case "OnOtherGhostVanish":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(7, out string r7);

                        retValue = ghost.OnOtherGhostVanish(req.References, r0, r1, r2, r7);
                    }
                    break;
                #endregion
                #region 選択肢イベント

                // TODO: OnChoiceSelect

                // TODO: OnChoiceSelectEx

                // TODO: OnChoiceEnter

                // TODO: OnChoiceTimeout

                // TODO: OnChoiceHover

                // TODO: OnAnchorSelect

                // TODO: OnAnchorSelectEx

                #endregion
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

        internal void Unload(Shiolink.Unload unload)
        {
            ghost.SaveData.Save();
        }

        internal static Response CreateOKResponse(string value)
        {
            var res = new Response(StatusCode.OK, NAME)
            {
                Charset = "UTF-8",

                Value = value
            };

            return res;
        }

        internal static Response CreateNoContentResponse()
        {
            var res = new Response(StatusCode.NO_CONTENT)
            {
                Charset = "UTF-8"
            };

            return res;
        }

        internal static Response CreateBadRequestResponse()
        {
            var res = new Response(StatusCode.BAD_REQUEST);

            return res;
        }

    }
}
