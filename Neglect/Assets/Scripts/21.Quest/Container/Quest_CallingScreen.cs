using GamePlay.Event;
using GamePlay.Phone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_CallingScreen : QuestBase
    {
        public CallingScreen calling;

        private PhoneControl phone;
        private IPhoneApplication app;
        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            var calls = PhoneUtil.InstantiateUI(calling, out phone);
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);
            phone.PhoneViewRotate(0);
            calls.name.text = "mom";
            calls.ClearAction += Complete;
            calls.IgnoreAction += Ignore;
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