using System;
using UnityEngine;

namespace Quest.Container
{
    /// <summary>
    /// 예시용
    /// </summary>
    public class Quest_GameRank : QuestBase
    {
        [Tooltip("목표 랭크")] public int goalRank;

        public void Awake()
        {
            eventData = new()
            {
                level = QuestLevel.Hard,
                isMainEvent = true,
            };

            eventData.name = questName;
        }

        public override void OnNext(object value)
        {
            if (value is int currentRank)
            {
                Debug.Log(currentRank);
                if (currentRank >= goalRank)
                {
                    Complete();
                }
                else
                {
                    Ignore();
                }
            }
        }
    }
}
