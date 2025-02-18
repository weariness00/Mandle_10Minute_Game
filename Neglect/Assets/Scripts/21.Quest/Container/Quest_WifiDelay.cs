using GamePlay.Event;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Container
{

    public class Quest_WifiDelay : QuestBase
    {
        public WiFiDelay WifiDelayPrefab;

        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            var wiFiDelay = PhoneUtil.InstantiateUI(WifiDelayPrefab);
            wiFiDelay.Complete += Complete; 
        }

        public override void Complete()
        {
            base.Complete();
        }
    }
}