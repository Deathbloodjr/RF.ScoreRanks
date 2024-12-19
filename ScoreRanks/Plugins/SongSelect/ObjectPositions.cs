using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScoreRanks.Plugins.SongSelect
{
    internal class ObjectPositions
    {
        public enum PositionId
        {
            P1Oni,
            P1Ura,
            P2Oni,
            P2Ura,
        }

        public enum PositionIdPosition
        {
            P1OniSelected,
            P1OniUnselected,
            P1UraSelected,
            P1UraUnselected,
            P2OniSelected,
            P2OniUnselected,
            P2UraSelected,
            P2UraUnselected,
        }

        public struct ScoreRankPosition
        {
            public Vector2 Position = Vector2.zero;
            public Vector2 Scale = Vector2.one;

            public ScoreRankPosition(Vector2 position, Vector2 scale)
            {
                Position = position;
                Scale = scale;
            }
        }

        public struct ScoreRankData
        {
            //DataConst.CrownType crown, EnsoData.EnsoLevelType level
            public int PlayerNum;
            public ScoreRank Rank;
            public EnsoData.EnsoLevelType Level;
            public PositionId PositionId { get { return GetPositionId(); } }

            public ScoreRankData(int playerNum, ScoreRank rank, EnsoData.EnsoLevelType level)
            {
                PlayerNum = playerNum;
                Rank = rank;
                Level = level;
            }

            PositionId GetPositionId()
            {
                if (Level == EnsoData.EnsoLevelType.Ura)
                {
                    if (PlayerNum == 0)
                    {
                        return PositionId.P1Ura;
                    }
                    else
                    {
                        return PositionId.P2Ura;
                    }
                }
                else
                {
                    if (PlayerNum == 0)
                    {
                        return PositionId.P1Oni;
                    }
                    else
                    {
                        return PositionId.P2Oni;
                    }
                }
            }
        }

        internal class PlayerScoreRankPositions
        {
            public static ScoreRankPosition GetScoreRankPosition(int playerNo, bool isUra, bool isSelected)
            {
                if (playerNo == 0)
                {
                    if (!isUra)
                    {
                        if (isSelected)
                        {
                            return GetScoreRankPosition(PositionIdPosition.P1OniSelected);

                        }
                        else
                        {
                            return GetScoreRankPosition(PositionIdPosition.P1OniUnselected);

                        }
                    }
                    else
                    {
                        if (isSelected)
                        {
                            return GetScoreRankPosition(PositionIdPosition.P1UraSelected);

                        }
                        else
                        {
                            return GetScoreRankPosition(PositionIdPosition.P1UraUnselected);
                        }
                    }
                }
                else if (playerNo == 1)
                {
                    if (!isUra)
                    {
                        if (isSelected)
                        {
                            return GetScoreRankPosition(PositionIdPosition.P2OniSelected);

                        }
                        else
                        {
                            return GetScoreRankPosition(PositionIdPosition.P2OniUnselected);

                        }
                    }
                    else
                    {
                        if (isSelected)
                        {
                            return GetScoreRankPosition(PositionIdPosition.P2UraSelected);

                        }
                        else
                        {
                            return GetScoreRankPosition(PositionIdPosition.P2UraUnselected);
                        }
                    }
                }
                else
                {
                    return new ScoreRankPosition(
                                new Vector2(0, 0),
                                new Vector2(1f, 1f));
                }
            }

            public static ScoreRankPosition GetScoreRankPosition(PositionIdPosition crown)
            {
                switch (crown)
                {
                    case PositionIdPosition.P1OniSelected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(-451, 62),
                                new Vector2(1f, 1f));
                    case PositionIdPosition.P1OniUnselected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(-370, 20),
                                new Vector2(0.8f, 0.8f));
                    case PositionIdPosition.P1UraSelected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(-451, 7),
                                new Vector2(1f, 1f));
                    case PositionIdPosition.P1UraUnselected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(-370, -20),
                                new Vector2(0.8f, 0.8f));
                    case PositionIdPosition.P2OniSelected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(395, 110),
                                new Vector2(1f, 1f));
                    case PositionIdPosition.P2OniUnselected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(407, 20),
                                new Vector2(0.8f, 0.8f));
                    case PositionIdPosition.P2UraSelected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(395, 55),
                                new Vector2(1f, 1f));
                    case PositionIdPosition.P2UraUnselected:
                        return new ScoreRankPosition( // From Crown position
                                new Vector2(407, -20),
                                new Vector2(0.8f, 0.8f));
                    default:
                        return new ScoreRankPosition(
                                new Vector2(0, 0),
                                new Vector2(1f, 1f));
                }
            }

            public static ScoreRankPosition GetScoreRankPosition(PositionId id, bool isSelected)
            {
                switch (id)
                {
                    case PositionId.P1Oni: return isSelected ? GetScoreRankPosition(PositionIdPosition.P1OniSelected) : GetScoreRankPosition(PositionIdPosition.P1OniUnselected);
                    case PositionId.P1Ura: return isSelected ? GetScoreRankPosition(PositionIdPosition.P1UraSelected) : GetScoreRankPosition(PositionIdPosition.P1UraUnselected);
                    case PositionId.P2Oni: return isSelected ? GetScoreRankPosition(PositionIdPosition.P2OniSelected) : GetScoreRankPosition(PositionIdPosition.P2OniUnselected);
                    case PositionId.P2Ura: return isSelected ? GetScoreRankPosition(PositionIdPosition.P2UraSelected) : GetScoreRankPosition(PositionIdPosition.P2UraUnselected);
                }

                return new ScoreRankPosition(new Vector2(0, 0),
                                         new Vector2(1f, 1f));
            }
        }
    }
}
