using HarmonyLib;
using Scripts.OutGame.SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Scripts.OutGame.SongSelect.UiSongButton;
using UnityEngine.UI;
using UnityEngine;
using Scripts.UserData;
using static MusicDataInterface;
using Scripts.Scene;

namespace ScoreRanks.Plugins
{
    class ScoreRankSongSelect
    {
        [HarmonyPatch(typeof(UiSongButton))]
        [HarmonyPatch(nameof(UiSongButton.SetCrown))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static void UiSongButton_SetCrown_Prefix(UiSongButton __instance, int id)
        {
            SetScoreRankIcons(__instance, id);
        }

        public static void SetScoreRankIcons(UiSongButton __instance, int id)
        {
            Vector2 scoreRankScale = new Vector2(0.25f, 0.25f);
            Vector2 scoreRankPosition = new Vector2(0, -25);

            Vector2 newCrownPosition = new Vector2(0, 2);
            Vector2 newBgPosition = new Vector2(0, 9);
            Vector2 backgroundBgPosition = new Vector2(0, 0);

            for (CrownObjTypes i = 0; i <= CrownObjTypes.Ura; i++)
            {
                if (i == CrownObjTypes.Ura && !__instance.hasUra)
                {
                    continue;
                }

                var crown = __instance.crowns[(int)i];

                // Set up ScoreRank object and image
                string scoreRankObjName = "ScoreRank";

                GameObject scoreRank = crown.transform.Find(scoreRankObjName)?.gameObject;
                if (scoreRank is null)
                {
                    scoreRank = new GameObject(scoreRankObjName);
                }
                scoreRank.transform.SetParent(crown.transform);
                scoreRank.transform.localPosition = scoreRankPosition;
                scoreRank.transform.localScale = scoreRankScale;
                var scoreRankImage = scoreRank.GetComponent<Image>();
                if (scoreRankImage is null)
                {
                    scoreRankImage = scoreRank.AddComponent<Image>();
                }
                scoreRankImage.raycastTarget = false;
                var sprite = GetScoreRankSprite(0, id, (EnsoData.EnsoLevelType)i);
                if (sprite is not null)
                {
                    scoreRankImage.sprite = sprite;
                }

                // Set up Crown object
                GameObject crownObj = crown.transform.Find("Crown")?.gameObject;
                crownObj.transform.localPosition = newCrownPosition;

                // Set up backgroundBg object
                GameObject newObj = crown.transform.Find("BgBackground")?.gameObject;
                if (newObj is null)
                {
                    GameObject bgObj = crown.transform.Find("Bg")?.gameObject;
                    newObj = UnityEngine.Object.Instantiate(bgObj, crown);
                    newObj.name = "BgBackground";
                    newObj.transform.localPosition = backgroundBgPosition;
                    newObj.transform.SetSiblingIndex(0);

                    bgObj.transform.localPosition = newBgPosition;
                }
 
            }
        }

        static Sprite GetScoreRankSprite(int playerNo, int uniqueId, EnsoData.EnsoLevelType difficulty)
        {
            if (MusicDataUtility.GetNormalRecordInfo(playerNo, uniqueId, difficulty, out var record))
            {
                var musicInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.GetInfoByUniqueId(uniqueId);
                var scoreRank = ScoreRankUtility.GetScoreRank(record.normalHiScore.score, musicInfo.Id, difficulty);
                return ScoreRankUtility.GetSprite(scoreRank, ScoreRankSpriteVersion.Small);
            }
            else
            {
                Logger.Log("Couldn't get record for song uid: " + uniqueId + " difficulty: " + difficulty.ToString());
            }
            return null;
        }

        static Sprite GetScoreRankSprite(int playerNo, MusicInfoAccesser musicInfo, EnsoData.EnsoLevelType difficulty)
        {
            if (MusicDataUtility.GetNormalRecordInfo(playerNo, musicInfo.UniqueId, difficulty, out var record))
            {
                var scoreRank = ScoreRankUtility.GetScoreRank(record.normalHiScore.score, musicInfo.Id, difficulty);
                return ScoreRankUtility.GetSprite(scoreRank, ScoreRankSpriteVersion.Small);
            }
            else
            {
                Logger.Log("Couldn't get record for song uid: " + musicInfo.UniqueId + " difficulty: " + difficulty.ToString());
            }
            return null;
        }

        [HarmonyPatch(typeof(UiSongButtonDifficulty))]
        [HarmonyPatch(nameof(UiSongButtonDifficulty.Setup))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UiSongButtonDifficulty_Setup_Postfix(UiSongButtonDifficulty __instance, MusicInfoAccesser item)
        {
            //SceneDataExchanger.Is2PMode;
            if (SceneDataExchanger.Is2PMode)
            {
                return;
            }

            if (item.Stars[(int)__instance.difficulty] == 0)
            {
                return;
            }

            GameObject scoreRank = __instance.transform.Find("ScoreRank")?.gameObject;
            Image scoreRankImage;
            if (scoreRank is null)
            {
                scoreRank = new GameObject("ScoreRank");
                scoreRank.transform.SetParent(__instance.transform);
                scoreRank.AddComponent<Image>();
                scoreRank.transform.localPosition = new Vector2(85, -24);
                scoreRank.transform.localScale = new Vector2(0.5f, 0.5f);
            }
            scoreRankImage = scoreRank.GetComponent<Image>();

            var sprite = GetScoreRankSprite(0, item, __instance.difficulty);
            if (sprite is not null)
            {
                scoreRankImage.sprite = sprite;
            }
        }

    }
}
