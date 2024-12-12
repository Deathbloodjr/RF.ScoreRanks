using HarmonyLib;
using NijiiroScoring.Plugins;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreRanks.Plugins
{
    enum ScoreRank
    {
        None,
        WhiteIki,
        BronzeIki,
        SilverIki,
        GoldMiyabi,
        PinkMiyabi,
        PurpleMiyabi,
        Kiwami
    }

    internal class ScoreRankData
    {
        public int CurrentScore = 0;
        public int ScoreRankValue = -1;
        public ScoreRank CurrentRank = ScoreRank.None;

        public void Reset()
        {
            CurrentScore = 0;
            CurrentRank = ScoreRank.None;
        }

        public void Reset(int newScoreRankValue)
        {
            Reset();
            ScoreRankValue = newScoreRankValue;
        }
    }

    internal class ScoreRankPatch
    {
        static ScoreRankData Player1 = new ScoreRankData();
        static ScoreRankData Player2 = new ScoreRankData();

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcLoading))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static void EnsoGameManager_ProcLoading_Prefix(EnsoGameManager __instance)
        {
            if (NijiroScoringPatch.IsEnabled)
            {
                var musicInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.GetInfoByUniqueId(__instance.settings.musicUniqueId);
                var points = SongDataManager.GetSongDataPoints(musicInfo.Id, __instance.settings.ensoPlayerSettings[0].courseType);
                if (points != null)
                {
                    Player1.ScoreRankValue = points.ScoreRank;
                }
                else
                {
                    Player1.ScoreRankValue = -1;
                }
                if (__instance.settings.ensoPlayerSettings.Length > 1)
                {
                    var p2points = SongDataManager.GetSongDataPoints(musicInfo.Id, __instance.settings.ensoPlayerSettings[1].courseType);
                    if (p2points != null)
                    {
                        Player1.ScoreRankValue = p2points.ScoreRank;
                    }
                    else
                    {
                        Player1.ScoreRankValue = -1;
                    }
                }
                Player1.CurrentScore = 0;
                Player2.CurrentScore = 0;
            }
            else
            {
                Player1.ScoreRankValue = -1;
                Player2.ScoreRankValue = -1;
            }
        }

        [HarmonyPatch(typeof(ScorePlayer))]
        [HarmonyPatch(nameof(ScorePlayer.SetAddScorePool))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void ScorePlayer_SetAddScorePool_Prefix(ScorePlayer __instance, int index, ref int score)
        {
            if (!NijiroScoringPatch.IsEnabled)
            {
                return;
            }
            
            ScoreRankData curPlayer = __instance.playerNo == 0 ? Player1 : Player2;

            curPlayer.CurrentScore += score;

            Logger.Log("P" + (__instance.playerNo + 1) + " CurrentScore: " + curPlayer.CurrentScore.ToString());

            var newRank = GetScoreRank(curPlayer);
            if (newRank != curPlayer.CurrentRank)
            {
                curPlayer.CurrentRank = newRank;
                Logger.Log("P" + (__instance.playerNo + 1) + "ScoreRank: " + curPlayer.CurrentRank.ToString());
                CreateEnsoScoreRankIcon(curPlayer.CurrentRank, 0);
            }

            return;
        }

        [HarmonyPatch(typeof(ResultPlayer))]
        [HarmonyPatch(nameof(ResultPlayer.DisplayCrown))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void ResultPlayer_DisplayCrown_Postfix(ResultPlayer __instance)
        {
            var parent = GameObject.Find("BaseMain");

            Vector2 DesiredPosition = new Vector2(-267, 0);
            Vector2 RealPosition = new Vector2(DesiredPosition.x + 868, DesiredPosition.y + 224);

            var imageObj = AssetUtility.CreateImageChild(parent, "ScoreRankResult", RealPosition, Path.Combine(Plugin.Instance.ConfigScoreRankAssetFolderPath.Value, "Big", Player1.CurrentRank.ToString() + ".png"));
            var image = imageObj.GetComponent<Image>();
            var imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(imageObj, 1, true));

            //DesiredPosition = new Vector2(-442, 190);
            //RealPosition = new Vector2(DesiredPosition.x + 868, DesiredPosition.y + 224);

            //var imageObj2 = AssetUtility.CreateImageChild(parent, "ScoreRankResult", RealPosition, Path.Combine(Plugin.Instance.ConfigScoreRankAssetFolderPath.Value, "Big", currentP1Rank.ToString() + ".png"));
            //var image2 = imageObj2.GetComponent<Image>();
            //var imageColor2 = image2.color;
            //imageColor2.a = 0;
            //image2.color = imageColor2;

            //Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(imageObj2, 1, true));
        }


        public static void ResetScore()
        {
            Player1.Reset();
            Player2.Reset();
        }

        public static ScoreRank GetScoreRank(ScoreRankData player)
        {
            return GetScoreRank(player.CurrentScore, player.ScoreRankValue);
        }

        public static ScoreRank GetScoreRank(int score, int maxScore)
        {
            var ratio = (float)score / (float)maxScore;
            if (ratio >= 1f)
            {
                return ScoreRank.Kiwami;
            }
            else if (ratio >= 0.95f)
            {
                return ScoreRank.PurpleMiyabi;
            }
            else if (ratio >= 0.9f)
            {
                return ScoreRank.PinkMiyabi;
            }
            else if (ratio >= 0.8f)
            {
                return ScoreRank.GoldMiyabi;
            }
            else if (ratio >= 0.7f)
            {
                return ScoreRank.SilverIki;
            }
            else if (ratio >= 0.6f)
            {
                return ScoreRank.BronzeIki;
            }
            else if (ratio >= 0.5f)
            {
                return ScoreRank.WhiteIki;
            }
            else
            {
                return ScoreRank.None;
            }
        }

        public static void CreateEnsoScoreRankIcon(ScoreRank scoreRank, int playerNo)
        {
            if (GameObject.Find("DaniDojo") == null)
            {
                Plugin.Instance.StartCoroutine(CreateEnsoScoreRankAnimation(scoreRank, playerNo));
            }
        }

        public static IEnumerator CreateEnsoScoreRankAnimation(ScoreRank scoreRank, int playerNo)
        {
            var canvasFgObject = GameObject.Find("CanvasFg");

            Vector2 MainPosition = GetScoreRankPosition(-905, 305);
            Vector2 DesiredPosition = new Vector2(-905, 305);

            // This position is changed at runtime, but the desired location is -920, 300
            // Adding 1920/2 or 1080/2 will put it at that location
            var scoreRankObject = AssetUtility.CreateImageChild(canvasFgObject, "ScoreRank", MainPosition + new Vector2(0, -50), Path.Combine(Plugin.Instance.ConfigScoreRankAssetFolderPath.Value, "Big", scoreRank.ToString() + ".png"));
            var image = scoreRankObject.GetOrAddComponent<Image>();
            var imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;

            Plugin.Instance.StartCoroutine(AssetUtility.MoveOverSeconds(scoreRankObject, DesiredPosition, 0.25f));
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(scoreRankObject, 0.25f, true));
            yield return new WaitForSeconds(0.25f);

            // Grow and shrink over 200 ms?

            yield return new WaitForSeconds(0.2f);

            // Wait 2 seconds before moving up and disappearing
            yield return new WaitForSeconds(2);

            Plugin.Instance.StartCoroutine(AssetUtility.MoveOverSeconds(scoreRankObject, DesiredPosition + new Vector2(0, 50), 0.25f));
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(scoreRankObject, 0.25f, false));
            yield return new WaitForSeconds(0.5f);

            GameObject.Destroy(scoreRankObject);

        }

        private static Vector2 GetScoreRankPosition(int x, int y)
        {
            return new Vector2(x + (1920 / 2), y + (1080 / 2));
        }
        private static Vector2 GetScoreRankPosition(Vector2 input)
        {
            return new Vector2(input.x + (1920 / 2), input.y + (1080 / 2));
        }
    }
}
