using Scripts.OutGame.Common;
using Scripts.OutGame.SongSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreRanks.Plugins.SongSelect
{
    internal class SpriteInitialization
    {
        public static Dictionary<EnsoData.EnsoLevelType, Sprite> DifficultySprites = new Dictionary<EnsoData.EnsoLevelType, Sprite>();

        public static bool IsInitialized()
        {
            ClearNullsFromDiffDictionary();

            if (DifficultySprites.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static void InitializeDifficultySprites(AllDifficultyScoreBoard __instance)
        {
            ClearNullsFromDiffDictionary();

            // Check if everything's already initialized
            if (DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Easy) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Normal) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Hard) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Mania) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Ura))
            {
                return;
            }

            Dictionary<string, EnsoData.EnsoLevelType> GameObjectNameToEnsoLevel = new Dictionary<string, EnsoData.EnsoLevelType>()
            {
                { "ScoreComponent", EnsoData.EnsoLevelType.Easy },
                { "ScoreComponent (1)", EnsoData.EnsoLevelType.Normal },
                { "ScoreComponent (2)", EnsoData.EnsoLevelType.Hard },
                { "ScoreComponent (3)", EnsoData.EnsoLevelType.Mania },
                { "ScoreComponent (4)", EnsoData.EnsoLevelType.Ura },
            };

            var children = __instance.scorePanels;
            for (int i = 0; i < children.Count; i++)
            {
                if (!DifficultySprites.ContainsKey(GameObjectNameToEnsoLevel[children[i].name]))
                {
                    var iconCourse = children[i].transform.FindChild("IconCourse");
                    if (iconCourse != null)
                    {
                        Sprite sprite = iconCourse.GetComponent<Image>().sprite;
                        DifficultySprites.Add(GameObjectNameToEnsoLevel[children[i].name], sprite);
                        Logger.Log("Sprite added for difficulty: " + GameObjectNameToEnsoLevel[children[i].name].ToString());
                    }
                }
            }
        }

        private static void ClearNullsFromDiffDictionary()
        {
            foreach (var item in DifficultySprites)
            {
                if (item.Value == null)
                {
                    DifficultySprites.Remove(item.Key);
                }
            }
        }
    }
}
