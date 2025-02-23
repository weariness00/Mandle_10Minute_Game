using GamePlay.Event;
using GamePlay.Phone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quest.Container
{
    public class Quest_CallingScreen : QuestBase
    {
        public CallingScreen phoneCallScreenPrefab;

        private PhoneControl phone;
        private IPhoneApplication app;
        
        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            var calls = PhoneUtil.InstantiateUI(phoneCallScreenPrefab, out phone);
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);
            phone.PhoneViewRotate(0);
            app.SetActiveBackground(true);
            
            calls.name.text = eventData.textArray.Length == 0 ? "mom" : eventData.textArray[0];

            if (eventData.extraDataIDArray.Length >= 1) isReverse = eventData.extraDataIDArray[0] == -99;
            calls.ClearAction += isReverse ? Ignore : Complete;
            calls.IgnoreAction += isReverse ? Complete : Ignore;
        }
        public override void Complete()
        {
            base.Complete();
        }

        public override void Ignore()
        {
            phone.applicationControl.OpenApp(app);
            base.Ignore();
        }
    }
}