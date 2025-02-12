﻿using System;
using UnityEngine;

namespace Quest.Container
{
    /// <summary>
    /// 예시용
    /// </summary>
    public class Quest_GameRank : QuestBase
    {
        [Tooltip("목표 랭크")] public int goalRank;

        public override void OnNext(object value)
        {
            if (value is int currentRank)
            {
                Debug.Log(currentRank);
                if (currentRank >= goalRank)
                {
                    Complete();
                }
            }
        }
    }
}
