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
        private PhoneControl phone;
        public override void OnNext(object value)
        {
            
        }

        public override void Play()
        {
            base.Play();
            var Alarm = PhoneUtil.InstantiateUI(questPrefab, out var phone_);

            phone = phone_;
            Alarm.complete += Complete;
            Alarm.ignore += Ignore;
            Alarm.TimeSet("10:00");

            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);

            phone.FadeIn(3, Color.black);
            phone.PhoneViewRotate(0);
            PhoneUtil.currentPhone.PhoneVibration();
        }

        public override void Ignore()
        {
            base.Ignore();
            phone.applicationControl.OpenApp(app);
        }


        public override void Complete()
        {
            base.Complete();
            phone.applicationControl.OpenApp(app);
        }
    }
}