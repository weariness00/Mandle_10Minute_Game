using GamePlay.Event;
using GamePlay.Phone;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_Alarm : QuestBase
    {
        public Alarm questPrefab;
        private IPhoneApplication app;
        public override void OnNext(object value)
        {
            
        }

        public override void Play()
        {
            base.Play();
            var Alarm = PhoneUtil.InstantiateUI(questPrefab, out var phone);

            Alarm.complete += Complete;
            Alarm.ignore += Ignore;
            Alarm.TimeSet("10:00");

            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);
            phone.PhoneViewRotate(0);
            PhoneUtil.currentPhone.PhoneVibration();
        }

        public override void Ignore()
        {
            base.Ignore();


        }


        public override void Complete()
        {
            base.Complete();

        }
    }
}