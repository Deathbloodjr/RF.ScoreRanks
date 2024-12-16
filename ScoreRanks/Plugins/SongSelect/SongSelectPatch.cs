using HarmonyLib;
using Scripts.OutGame.SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreRanks.Plugins.SongSelect
{
    internal class SongSelectPatch
    {
        static Dictionary<UiSongButton, ButtonScoreRankObject> UiSongButtonObjects = new Dictionary<UiSongButton, ButtonScoreRankObject>();
        static ButtonScoreRankObject UiSongCenterButtonObject;

        [HarmonyPatch(typeof(UiSongScroller))]
        [HarmonyPatch(nameof(UiSongScroller.Setup))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UiSongScroller_Setup_Postfix(UiSongScroller __instance)
        {
            ClearUiSongButtonDictionary();
        }

        [HarmonyPatch(typeof(AllDifficultyScoreBoard))]
        [HarmonyPatch(nameof(AllDifficultyScoreBoard.Setup))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void AllDifficultyScoreBoard_Setup_Postfix(AllDifficultyScoreBoard __instance)
        {
            SpriteInitialization.InitializeDifficultySprites(__instance);
        }

        private static void ClearUiSongButtonDictionary()
        {
            foreach (var item in UiSongButtonObjects)
            {
                if (item.Key == null ||
                    item.Value == null)
                {
                    UiSongButtonObjects.Remove(item.Key);
                }
            }
        }

        [HarmonyPatch(typeof(UiSongCenterButton))]
        [HarmonyPatch(nameof(UiSongCenterButton.ShrinkAsync))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool UiSongCenterButton_ShrinkAsync_Prefix(UiSongCenterButton __instance)
        {
            if (UiSongCenterButtonObject != null)
            {
                UiSongCenterButtonObject.ShrinkScoreRankss();
            }
            return true;
        }

        [HarmonyPatch(typeof(UiSongCenterButton))]
        [HarmonyPatch(nameof(UiSongCenterButton.ExpandAsync))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool UiSongCenterButton_ExpandAsync_Prefix(UiSongCenterButton __instance)
        {
            if (UiSongCenterButtonObject != null)
            {
                UiSongCenterButtonObject.ExpandScoreRankss();
            }
            return true;
        }

        [HarmonyPatch(typeof(UiSongCenterButton))]
        [HarmonyPatch(nameof(UiSongCenterButton.Setup))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UiSongCenterButton_Setup_Postfix(UiSongCenterButton __instance, MusicDataInterface.MusicInfoAccesser item)
        {
            if (UiSongCenterButtonObject == null || !UiSongCenterButtonObject.IsInitialized())
            {
                UiSongCenterButtonObject = new ButtonScoreRankObject(__instance);
            }
            Plugin.Instance.StartCoroutine(UiSongCenterButtonObject.ChangeScoreRanks(item));
        }

        [HarmonyPatch]
        public class UiSongButtonSetupPatch
        {
            static System.Reflection.MethodBase TargetMethod()
            {
                return typeof(UiSongButton).GetMethod(nameof(UiSongButton.Setup)).MakeGenericMethod(typeof(SongButton));
            }

            static void Postfix(UiSongButton __instance, SongButton model)
            {
                if (!UiSongButtonObjects.ContainsKey(__instance))
                {
                    UiSongButtonObjects.Add(__instance, new ButtonScoreRankObject(__instance));
                }
                Plugin.Instance.StartCoroutine(UiSongButtonObjects[__instance].ChangeScoreRanks(model.Value));
            }
        }
    }
}
