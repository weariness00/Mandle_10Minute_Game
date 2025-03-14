﻿using GamePlay;

namespace Quest
{
    public class Quest_Tired : QuestBase
    {
        public int repeatCount = 2;
        public float duration = 2f;
        
        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            if (eventData.extraDataIDArray.Length > 1) isLoop = eventData.extraDataIDArray[1] == -45;

            GameManager.Instance.realVolumeControl.Tired(repeatCount,duration);
            Complete();
        }
    }
}

