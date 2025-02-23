using GamePlay;
using System;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_GameClear : QuestBase
    {
        public void Awake()
        {
            eventData = new()
            {
                name = questName,
                isMainEvent = true,
                level = QuestLevel.Hard,
            };
        }

        public override void OnNext(object value)
        {
            if (value is float time)
            {
                if (GameManager.Instance.playTimer.Max <= time)
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

