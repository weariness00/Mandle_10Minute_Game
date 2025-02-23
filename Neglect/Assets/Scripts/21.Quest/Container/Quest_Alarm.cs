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

        private Alarm alarm;
        private IPhoneApplication app;
        private PhoneControl phone;
        public override void OnNext(object value)
        {
            
        }

        public override void Play()
        {
            base.Play();
            alarm = PhoneUtil.InstantiateUI(questPrefab, out var phone_);

            phone = phone_;
            alarm.complete += Complete;
            alarm.ignore += Ignore;
            alarm.TimeSet("10:00");

            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);

            phone.FadeOut(0.1f, Color.black);
            phone.PhoneViewRotate(0, ()=> phone.FadeIn(1f, Color.black));
            
            
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

        public override void Failed()
        {
            base.Failed();
            if(alarm) Destroy(alarm.gameObject);
        }
    }
}