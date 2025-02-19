using GamePlay.Event;
using GamePlay.Phone;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Container
{

    public class Quest_WifiDelay : QuestBase
    {
        public WiFiDelay WifiDelayPrefab;
        
        private PhoneControl phone;
        private IPhoneApplication app;

        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            var wiFiDelay = PhoneUtil.InstantiateUI(WifiDelayPrefab);
            wiFiDelay.Complete += Complete; 
            
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);
            phone.PhoneViewRotate(0);
        }

        public override void Complete()
        {
            base.Complete();
            phone.applicationControl.OpenApp(app);
        }

        public override void Ignore()
        {
            base.Ignore();
            phone.applicationControl.OpenApp(app);
        }
    }
}