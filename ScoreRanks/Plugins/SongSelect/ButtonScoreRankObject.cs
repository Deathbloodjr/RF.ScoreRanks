using Scripts.OutGame.SongSelect;
using Scripts.UserData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static ScoreRanks.Plugins.SongSelect.ObjectPositions;

namespace ScoreRanks.Plugins.SongSelect
{
    internal class ButtonScoreRankObject
    {
        public Dictionary<PositionId, GameObject> ScoreRankGameObjects = new Dictionary<PositionId, GameObject>();
        bool isSelected;

        public ButtonScoreRankObject(UiSongButton parent)
        {
            this.isSelected = false;
            InitializeScoreRankGameObjects(parent.gameObject);
        }

        public ButtonScoreRankObject(UiSongCenterButton parent)
        {
            this.isSelected = true;
            InitializeScoreRankGameObjects(parent.gameObject);
        }

        public bool IsInitialized()
        {
            foreach (var gameObjects in ScoreRankGameObjects)
            {
                if (gameObjects.Value == null)
                {
                    return false;
                }
            }

            return true;
        }

        void InitializeScoreRankGameObjects(GameObject parent)
        {


            GameObject ScoreRankParent = new GameObject("ScoreRank");
            ScoreRankParent.transform.SetParent(parent.transform);
            ScoreRankParent.transform.localPosition = Vector2.zero;
            InitializeScoreRankGameObject(PositionId.P1Oni, ScoreRankParent);
            InitializeScoreRankGameObject(PositionId.P1Ura, ScoreRankParent);
            InitializeScoreRankGameObject(PositionId.P2Oni, ScoreRankParent);
            InitializeScoreRankGameObject(PositionId.P2Ura, ScoreRankParent);
        }

        void InitializeScoreRankGameObject(PositionId scoreRankId, GameObject parent)
        {
            GameObject scoreRankObj = new GameObject(scoreRankId.ToString());
            scoreRankObj.transform.SetParent(parent.transform);
            ScoreRankPosition pos = PlayerScoreRankPositions.GetScoreRankPosition(scoreRankId, isSelected);
            scoreRankObj.transform.localPosition = pos.Position;
            scoreRankObj.transform.localScale = pos.Scale;

            GameObject scoreRank = new GameObject("ScoreRank");
            scoreRank.transform.SetParent(scoreRankObj.transform);
            var scoreRankImage = scoreRank.AddComponent<Image>();
            scoreRankImage.raycastTarget = false;
            var scoreRankRect = scoreRank.GetComponent<RectTransform>();
            scoreRankRect.localPosition = new Vector2(0, 0);
            scoreRankRect.localScale = new Vector2(1f, 1f);

            GameObject diffObj = new GameObject("Difficulty");
            diffObj.transform.SetParent(scoreRankObj.transform);
            var diffImage = diffObj.AddComponent<Image>();
            diffImage.raycastTarget = false;
            var diffRect = diffObj.GetComponent<RectTransform>();
            diffRect.localPosition = new Vector2(10, -10);
            diffRect.localScale = new Vector2(0.7f, 0.7f);

            scoreRankObj.SetActive(false);

            ScoreRankGameObjects.Add(scoreRankId, scoreRankObj);
        }

        internal IEnumerator ChangeScoreRanks(MusicDataInterface.MusicInfoAccesser musicInfo)
        {
            if (musicInfo != null)
            {
                while (!SpriteInitialization.IsInitialized())
                {
                    yield return new WaitForEndOfFrame();
                }

                // I need to test this when I have a controller available
                var numPlayers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.playerNum;
                Logger.Log(numPlayers.ToString(), LogType.Debug);

                for (int i = 0; i < 1; i++)
                {
                    // Changing for player 2, but there's only 1 player
                    // Actually this numPlayers is incorrect anyway
                    //if (i == 1 && numPlayers == 1)
                    //{

                    //}
                    if (musicInfo.Stars.Length >= (int)EnsoData.EnsoLevelType.Ura &&
                        musicInfo.Stars[(int)EnsoData.EnsoLevelType.Ura] == 0)
                    {
                        ChangeScoreRank(new ScoreRankData(i, ScoreRank.None, EnsoData.EnsoLevelType.Ura));
                    }
                    else
                    {
                        MusicDataUtility.GetNormalRecordInfo(i, musicInfo.UniqueId, EnsoData.EnsoLevelType.Ura, out var ura);
                        var scoreRank = ScoreRankUtility.GetScoreRank(ura.normalHiScore.score, musicInfo.Id, EnsoData.EnsoLevelType.Ura);
                        ChangeScoreRank(new ScoreRankData(i, scoreRank, EnsoData.EnsoLevelType.Ura));
                    }

                    bool highScoreFound = false;
                    for (EnsoData.EnsoLevelType j = EnsoData.EnsoLevelType.Mania; j >= EnsoData.EnsoLevelType.Easy; j--)
                    {
                        MusicDataUtility.GetNormalRecordInfo(0, musicInfo.UniqueId, j, out var result);
                        highScoreFound = true;
                        var scoreRank = ScoreRankUtility.GetScoreRank(result.normalHiScore.score, musicInfo.Id, j);
                        ChangeScoreRank(new ScoreRankData(i, scoreRank, j));
                        break;
                    }

                    if (!highScoreFound)
                    {
                        ChangeScoreRank(new ScoreRankData(i, ScoreRank.None, EnsoData.EnsoLevelType.Num));
                    }
                }
            }
            else
            {
                ChangeScoreRank(new ScoreRankData(0, ScoreRank.None, EnsoData.EnsoLevelType.Num));
            }
        }

        void ChangeScoreRank(ScoreRankData data)
        {
            var positionId = data.PositionId;
            if (data.Rank == ScoreRank.None)
            {
                ScoreRankGameObjects[positionId].SetActive(false);
                return;
            }

            ScoreRankGameObjects[positionId].SetActive(true);

            var scoreRankObj = ScoreRankGameObjects[positionId].transform.FindChild("ScoreRank");
            var scoreRankSprite = ScoreRankUtility.GetSprite(data.Rank, ScoreRankSpriteVersion.Small);

            if (scoreRankSprite != null)
            {
                var scoreRankImage = scoreRankObj.GetComponent<Image>();
                scoreRankImage.sprite = ScoreRankUtility.GetSprite(data.Rank, ScoreRankSpriteVersion.Small);
                // I'm not sure if we need to set the sizeDelta each time
                var scoreRankRect = scoreRankObj.GetComponent<RectTransform>();
                scoreRankRect.sizeDelta = new Vector2(scoreRankImage.sprite.rect.width, scoreRankImage.sprite.rect.height);
            }

            var diffObj = ScoreRankGameObjects[positionId].transform.FindChild("Difficulty");
            if (data.Level == EnsoData.EnsoLevelType.Num || data.Rank == ScoreRank.None)
            {
                diffObj.gameObject.SetActive(false);
            }
            else if (SpriteInitialization.DifficultySprites.ContainsKey(data.Level) &&
                SpriteInitialization.DifficultySprites[data.Level] != null)
            {
                diffObj.gameObject.SetActive(true);
                var scoreRankImage = diffObj.GetComponent<Image>();
                scoreRankImage.sprite = SpriteInitialization.DifficultySprites[data.Level];
                // I'm not sure if we need to set the sizeDelta each time
                var scoreRankRect = diffObj.GetComponent<RectTransform>();
                scoreRankRect.sizeDelta = new Vector2(scoreRankImage.sprite.rect.width, scoreRankImage.sprite.rect.height);
            }
        }


        public void ExpandScoreRankss()
        {
            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P1Oni, PositionIdPosition.P1OniUnselected, PositionIdPosition.P1OniSelected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P1Ura, PositionIdPosition.P1UraUnselected, PositionIdPosition.P1UraSelected, 0.125f));

            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P2Oni, PositionIdPosition.P2OniUnselected, PositionIdPosition.P2OniSelected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P2Ura, PositionIdPosition.P2UraUnselected, PositionIdPosition.P2UraSelected, 0.125f));
        }

        public void ShrinkScoreRankss()
        {
            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P1Oni, PositionIdPosition.P1OniSelected, PositionIdPosition.P1OniUnselected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P1Ura, PositionIdPosition.P1UraSelected, PositionIdPosition.P1UraUnselected, 0.125f));

            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P2Oni, PositionIdPosition.P2OniSelected, PositionIdPosition.P2OniUnselected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveScoreRanks(PositionId.P2Ura, PositionIdPosition.P2UraSelected, PositionIdPosition.P2UraUnselected, 0.125f));
        }

        IEnumerator MoveScoreRanks(PositionId id, PositionIdPosition start, PositionIdPosition end, float duration)
        {
            if (ScoreRankGameObjects.ContainsKey(id))
            {
                GameObject scoreRankObj = ScoreRankGameObjects[id];

                float elapsedTime = 0f;
                ScoreRankPosition startPos = PlayerScoreRankPositions.GetScoreRankPosition(start);
                ScoreRankPosition endPos = PlayerScoreRankPositions.GetScoreRankPosition(end);

                while (elapsedTime < duration)
                {
                    scoreRankObj.transform.localPosition = Vector2.Lerp(startPos.Position, endPos.Position, elapsedTime / duration);
                    scoreRankObj.transform.localScale = Vector2.Lerp(startPos.Scale, endPos.Scale, elapsedTime / duration);

                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                scoreRankObj.transform.localPosition = endPos.Position;
                scoreRankObj.transform.localScale = endPos.Scale;
            }
        }
    }
}
