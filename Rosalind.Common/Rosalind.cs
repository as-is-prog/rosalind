﻿using Shiorose.Resource;
using Shiorose.Resource.ShioriEvent;
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
        /// <summary>
        /// バイト値1の文字列
        /// </summary>
        public static readonly string BYTE_1_STR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 1 });
        /// <summary>
        /// バイト値1の文字
        /// </summary>
        public static readonly char BYTE_1_CHAR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 1 }).First();
        /// <summary>
        /// バイト値2の文字列
        /// </summary>
        public static readonly string BYTE_2_STR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 2 });

        private static readonly string NAME = "Rosalind";
        private static readonly string VERSION = "0.1.6";
        private static readonly string CRAFTMAN = "as-is-prog";
        private static readonly string CRAFTMAN_W = "AS-IS";

        private static readonly string SHIORI_PROTOCOL_VERSION = "3.0";

        /// <summary>
        /// SHIORIの存在しているディレクトリ
        /// </summary>
        public static string ShioriDir { get; private set; }

        internal Ghost ghost;

        internal Rosalind(Load load)
        {
            ShioriDir = load.ShioriDir;
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
                        OnNOTIFYRequest(req);
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
                /* SHIORI Resource (has Ghost class)*/
                case "homeurl":
                    retValue = ghost.Homeurl;
                    break;
                case "username":
                    retValue = ghost.SaveData != null ? ghost.SaveData.UserName : "";
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
                //* SHIORI Resouce (has SHIORIResource class)*/
                case "sakura.recommendbuttoncaption":
                    retValue = ghost.Resource.SakuraRecommendButtonCaption();
                    break;
                case "kero.recommendbuttoncaption":
                    retValue = ghost.Resource.KeroRecommendButtonCaption();
                    break;
                case "sakura.portalbuttoncaption":
                    retValue = ghost.Resource.SakuraPortalButtonCaption();
                    break;
                case "kero.portalbuttoncaption":
                    retValue = ghost.Resource.KeroPortalButtonCaption();
                    break;
                case "updatebuttoncaption":
                    retValue = ghost.Resource.UpdateButtonCaption();
                    break;
                case "vanishbuttoncaption":
                    retValue = ghost.Resource.VanishButtonCaption();
                    break;
                case "readmebuttoncaption":
                    retValue = ghost.Resource.ReadmeButtonCaption();
                    break;
                case "vanishbuttonvisible":
                    retValue = ghost.Resource.VanishButtonVisible().ToString();
                    break;
                case "sakura.popupmenu.visible":
                    retValue = ghost.Resource.SakuraPopupMenuVisible().ToString();
                    break;
                case "kero.popupmenu.visible":
                    retValue = ghost.Resource.KeroPopupMenuVisible().ToString();
                    break;
                /* SHIORI event (supposes) */
                #region 起動・終了・切り替えイベント
                case "OnFirstBoot":
                    {
                        req.References.TryGetValue(0, out string r0);
                        int.TryParse(r0, out int vanishCount);
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
                case "OnChoiceSelect":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var otherId = req.References.Skip(1).Select(r => r.Value);

                        retValue = ghost.OnChoiceSelect(req.References, r0, otherId);
                    }
                    break;
                case "OnChoiceSelectEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        var extInfo = req.References.Skip(2).Select(r => r.Value);

                        retValue = ghost.OnChoiceSelectEx(req.References, r0, r1, extInfo);
                    }
                    break;
                // TODO: OnChoiceEnter [NOTIFY]
                case "OnChoiceTimeout":
                    {
                        req.References.TryGetValue(0, out string r0);

                        retValue = ghost.OnChoiceTimeout(req.References, r0);
                    }
                    break;
                case "OnChoiceHover":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        var extInfo = req.References.Skip(2).Select(r => r.Value);

                        retValue = ghost.OnChoiceHover(req.References, r0, r1, extInfo);
                    }
                    break;
                case "OnAnchorSelect":
                    {
                        req.References.TryGetValue(0, out string r0);

                        retValue = ghost.OnAnchorSelect(req.References, r0);
                    }
                    break;
                case "OnAnchorSelectEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        var extInfo = req.References.Skip(2).Select(r => r.Value);

                        retValue = ghost.OnAnchorSelectEx(req.References, r0, r1, extInfo);
                    }
                    break;
                #endregion
                #region サーフェスイベント
                // TODO: OnSurfaceChange [NOTIFY]
                case "OnSurfaceRestore":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnSurfaceRestore(req.References, r0, r1);
                    }
                    break;
                case "OnOtherSurfaceChange":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);

                        retValue = ghost.OnOtherSurfaceChange(req.References, r0, r1, r2, r3, r4, r5);
                    }
                    break;
                #endregion
                #region マウスイベント
                case "OnMouseClick":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseClick(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseClickEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseClickEx(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseDoubleClick":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseDoubleClick(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseDoubleClickEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseDoubleClickEx(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseMultipleClick":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);
                        req.References.TryGetValue(7, out string r7);

                        int.TryParse(r7, out int comboCount);
                        retValue = ghost.OnMouseMultipleClick(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6), comboCount);
                    }
                    break;
                case "OnMouseMultipleClickEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);
                        req.References.TryGetValue(7, out string r7);

                        int.TryParse(r7, out int comboCount);
                        retValue = ghost.OnMouseMultipleClickEx(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6), comboCount);
                    }
                    break;
                case "OnMouseUp":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseUp(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseUpEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseUpEx(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseDown":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseDown(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseDownEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseDownEx(req.References, r0, r1, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseMove":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseMove(req.References, r0, r1, r2, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseWheel":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseWheel(req.References, r0, r1, r2, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseEnterAll":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseEnterAll(req.References, r0, r1, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseLeaveAll":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseLeaveAll(req.References, r0, r1, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseEnter":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseEnter(req.References, r0, r1, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseLeave":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseLeave(req.References, r0, r1, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseDragStart":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseDragStart(req.References, r0, r1, r2, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseDragEnd":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseDragEnd(req.References, r0, r1, r2, r3, r4, r5, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseHover":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseHover(req.References, r0, r1, r3, r4, ShioriParamTypeUtil.StringToDeviceType(r6));
                    }
                    break;
                case "OnMouseGesture":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        var mousePos = r1.Split(BYTE_1_STR.ToCharArray());
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var newMousePos = r3.Split(BYTE_1_STR.ToCharArray());
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);

                        retValue = ghost.OnMouseGesture(req.References, r0, mousePos[0], mousePos[1], r2, newMousePos[0], newMousePos[1], r4, r5, r6);
                    }
                    break;
                #endregion
                #region インストールイベント
                case "OnInStallBegin":
                    retValue = ghost.OnInstallBegin();
                    break;
                case "OnInstallComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var installType = ShioriParamTypeUtil.StringToInstallType(r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);

                        retValue = ghost.OnInstallComplete(req.References, installType, r1, r2);
                    }
                    break;
                case "OnInstallCompleteEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var installTypes = r0.Split(BYTE_1_CHAR).Select(it => ShioriParamTypeUtil.StringToInstallType(it)).ToArray();
                        req.References.TryGetValue(1, out string r1);
                        var installNames = r1.Split(BYTE_1_CHAR).ToArray();
                        req.References.TryGetValue(2, out string r2);
                        var installPaths = r2.Split(BYTE_1_CHAR).ToArray();

                        retValue = ghost.OnInstallCompleteEx(req.References, installTypes, installNames, installPaths);
                    }
                    break;
                case "OnInstallFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var failureReason = ShioriParamTypeUtil.StringToInstallFailureReason(r0);

                        retValue = ghost.OnInstallFailure(req.References, failureReason);
                    }
                    break;
                case "OnInstallRefuse":
                    {
                        req.References.TryGetValue(0, out string r0);

                        retValue = ghost.OnInstallRefuse(req.References, r0);
                    }
                    break;
                case "OnInstallReroute":
                    {
                        req.References.TryGetValue(0, out string r0);

                        retValue = ghost.OnInstallReroute(req.References, r0);
                    }
                    break;
                #endregion
                #region ファイルドロップイベント
                case "OnFileDropping":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);

                        retValue = ghost.OnFileDropping(req.References, r0, r1);
                    }
                    break;
                case "OnDirectoryDrop":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        retValue = ghost.OnDirectoryDrop(req.References, r0, r1);
                    }
                    break;
                case "OnWallpaperChange":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnWallpaperChange(req.References, r0);
                    }
                    break;
                case "OnFileDropEx":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var filePaths = r0.Split(BYTE_1_CHAR).ToArray();
                        req.References.TryGetValue(1, out string r1);
                        retValue = ghost.OnFileDropEx(req.References, filePaths, r1);
                    }
                    break;
                case "OnUpdatedataCreating":
                    retValue = ghost.OnUpdatedataCreating();
                    break;
                case "OnUpdatedataCreated":
                    retValue = ghost.OnUpdatedataCreated();
                    break;
                case "OnNarCreating":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        var installType = ShioriParamTypeUtil.StringToInstallType(r2);
                        retValue = ghost.OnNarCreating(req.References, r0, r1, installType);
                    }
                    break;
                case "OnNarCreated":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        var installType = ShioriParamTypeUtil.StringToInstallType(r2);
                        retValue = ghost.OnNarCreated(req.References, r0, r1, installType);
                    }
                    break;
                #endregion
                #region URLドロップイベント
                case "OnURLDropping":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnURLDropping(req.References, r0);
                    }
                    break;
                case "OnURLDropped":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnURLDropped(req.References, r0);
                    }
                    break;
                case "OnURLDropFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        retValue = ghost.OnURLDropFailure(req.References, r0, r1);
                    }
                    break;
                case "OnURLQuery":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        retValue = ghost.OnURLQuery(req.References, r0, r1, r2);
                    }
                    break;
                #endregion
                #region ネットワーク更新イベント
                case "OnUpdateBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateBegin(req.References,r0, r1, updateType, updateReason);
                    }
                    break;
                case "OnUpdateReady":
                    {
                        req.References.TryGetValue(0, out string r0);
                        if (int.TryParse(r0, out int updateCount)) updateCount += 1;
                        req.References.TryGetValue(1, out string r1);
                        var updateFileNames = r1.Split(',').ToArray();
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateReady(req.References, updateCount, updateFileNames, updateType, updateReason);
                    }
                    break;
                case "OnUpdateComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var isUpdated = r0 == "changed";
                        req.References.TryGetValue(1, out string r1);
                        var updateFileNames = r1.Split(',').ToArray();
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateComplete(req.References,isUpdated, updateFileNames, updateType, updateReason);
                    }
                    break;
                case "OnUpdateFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateFailure(req.References, r0, r1, updateType, updateReason);
                    }
                    break;
                case "OnUpdate.OnDownloadBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        if (int.TryParse(r1, out int downloadProgressCount)) downloadProgressCount += 1;
                        req.References.TryGetValue(2, out string r2);
                        if (int.TryParse(r2, out int downloadTotalCount)) downloadTotalCount += 1;
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateOnDownloadBegin(req.References, r0, downloadProgressCount, downloadTotalCount, updateType, updateReason);
                    }
                    break;
                case "OnUpdate.OnMD5CompareBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateOnMD5CompareBegin(req.References, r0, r1, r2, updateType, updateReason);
                    }
                    break;
                case "OnUpdate.OnMD5CompareComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateOnMD5CompareComplete(req.References, r0, r1, r2, updateType, updateReason);
                    }
                    break;
                case "OnUpdate.OnMD5CompareFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateOnMD5CompareFailure(req.References, r0, r1, r2, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOtherBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateOtherBegin(req.References, r0, r1, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOtherReady":
                    {
                        req.References.TryGetValue(0, out string r0);
                        if (int.TryParse(r0, out int updateCount)) updateCount += 1;
                        req.References.TryGetValue(1, out string r1);
                        var updateFileNames = r1.Split(',').ToArray();
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateOtherReady(req.References, updateCount, updateFileNames, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOtherComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var isUpdated = r0 == "changed";
                        req.References.TryGetValue(1, out string r1);
                        var updateFileNames = r1.Split(',').ToArray();
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateOtherComplete(req.References, isUpdated, updateFileNames, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOtherFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateOtherFailure(req.References, r0, r1, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOther.OnDownloadBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        if (int.TryParse(r1, out int downloadProgressCount)) downloadProgressCount += 1;
                        req.References.TryGetValue(2, out string r2);
                        if (int.TryParse(r2, out int downloadTotalCount)) downloadTotalCount += 1;
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);

                        retValue = ghost.OnUpdateOtherOnDownloadBegin(req.References, r0, downloadProgressCount, downloadTotalCount, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOther.OnMD5CompareBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateOtherOnMD5CompareBegin(req.References, r0, r1, r2, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOther.OnMD5CompareComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateOtherOnMD5CompareComplete(req.References, r0, r1, r2, updateType, updateReason);
                    }
                    break;
                case "OnUpdateOther.OnMD5CompareFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateOtherOnMD5CompareFailure(req.References, r0, r1, r2, updateType, updateReason);
                    }
                    break;
                case "OnUpdateCheckComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        var isupdate = r0 == "changed";
                        req.References.TryGetValue(1, out string r1);
                        var updateFileNames = r1.Split(',');
                        req.References.TryGetValue(3, out string r3);
                        var updateType = ShioriParamTypeUtil.StringToUpdateType(r3);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateCheckComplete(req.References, isupdate, updateFileNames, updateType, updateReason);
                    }
                    break;
                case "OnUpdateCheckFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(4, out string r4);
                        var updateReason = ShioriParamTypeUtil.StringToUpdateReason(r4);
                        retValue = ghost.OnUpdateCheckFailure(req.References, r0, updateReason);
                    }
                    break;
                case "OnUpdateResult":
                    retValue = ghost.OnUpdateResult(req.References, req.References);
                    break;
                case "OnUpdateResultExplorer":
                    retValue = ghost.OnUpdateResultExplorer(req.References, req.References);
                    break;
                #endregion
                #region 時計合わせイベント
                case "OnSNTPBegin":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnSNTPBegin(req.References, r0);
                    }
                    break;
                case "OnSNTPCompare":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        DateTime? serverTime = null;
                        try
                        {
                            var st = r1.Split(',').Select(n => int.Parse(n)).ToArray();
                            serverTime = new DateTime(st[0], st[1], st[2], st[3], st[4], st[5], st[6]);
                        }
                        catch { }
                        req.References.TryGetValue(2, out string r2);
                        DateTime? localTime = null;
                        try
                        {
                            var st = r2.Split(',').Select(n => int.Parse(n)).ToArray();
                            localTime = new DateTime(st[0], st[1], st[2], st[3], st[4], st[5], st[6]);
                        }
                        catch { }
                        req.References.TryGetValue(3, out string r3);
                        int.TryParse(r3, out int diffSecond);
                        req.References.TryGetValue(4, out string r4);
                        int.TryParse(r4, out int diffMSecond);
                        retValue = ghost.OnSNTPCompare(req.References, r0, serverTime, localTime, diffSecond, diffMSecond);
                    }
                    break;
                case "OnSNTPCorrect":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        DateTime? serverTime = null;
                        try
                        {
                            var st = r1.Split(',').Select(n => int.Parse(n)).ToArray();
                            serverTime = new DateTime(st[0], st[1], st[2], st[3], st[4], st[5], st[6]);
                        }
                        catch { }
                        req.References.TryGetValue(2, out string r2);
                        DateTime? localTime = null;
                        try
                        {
                            var st = r2.Split(',').Select(n => int.Parse(n)).ToArray();
                            localTime = new DateTime(st[0], st[1], st[2], st[3], st[4], st[5], st[6]);
                        }
                        catch { }
                        req.References.TryGetValue(3, out string r3);
                        int.TryParse(r3, out int diffSecond);
                        retValue = ghost.OnSNTPCorrect(req.References, r0, serverTime, localTime, diffSecond);
                    }
                    break;
                case "OnSNTPFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        retValue = ghost.OnSNTPFailure(req.References, r0);
                    }
                    break;
                #endregion
                #region メールチェックイベント
                case "OnBIFFBegin":
                    {
                        req.References.TryGetValue(2, out string r2);
                        retValue = ghost.OnBIFFBegin(req.References, r2);
                    }
                    break;
                case "OnBIFFComplete":
                    {
                        req.References.TryGetValue(0, out string r0);
                        int.TryParse(r0, out int spoolMailCount);
                        req.References.TryGetValue(1, out string r1);
                        int.TryParse(r1, out int spoolMailByte);
                        req.References.TryGetValue(2, out string r2);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        req.References.TryGetValue(5, out string r5);
                        req.References.TryGetValue(6, out string r6);
                        req.References.TryGetValue(7, out string r7);
                        var senderAndTitle = r7.Split(BYTE_1_CHAR);
                        string sender = null;
                        string title = null;
                        try
                        {
                            sender = senderAndTitle[0];
                            title = senderAndTitle[1];
                        }
                        catch { }
                        retValue = ghost.OnBIFFComplete(req.References,spoolMailCount, spoolMailByte, r2, r3, r4, r5, r6, sender, title);
                    }
                    break;
                case "OnBIFFFailure":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(2, out string r2);
                        retValue = ghost.OnBIFFFailure(req.References, r0, r2);
                    }
                    break;
                #endregion

                #region 単体イベント
                case "OnKeyPress":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        req.References.TryGetValue(2, out string r2);
                        int.TryParse(r2, out int pressCount);
                        req.References.TryGetValue(3, out string r3);
                        req.References.TryGetValue(4, out string r4);
                        retValue = ghost.OnKeyPress(req.References, r0, r1, pressCount, r3, r4);
                    }
                    break;
                case "OnTextDrop":
                    {
                        req.References.TryGetValue(0, out string r0);
                        req.References.TryGetValue(1, out string r1);
                        retValue = ghost.OnTextDrop(req.References, string.Join("\n", r0.Split(BYTE_1_CHAR)), r1);
                    }
                    break;
                
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

        private void OnNOTIFYRequest(Request req)
        {
            switch (req.ID)
            {
                case "OnInitialize":
                    ghost.OnInitialize();
                    break;
                case "OnDestroy":
                    ghost.OnDestroy();
                    break;
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
