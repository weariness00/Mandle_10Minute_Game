using GamePlay.Event;
using GamePlay.Phone;
using Manager;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_CallConversation : QuestBase
    {
        [Header("복사할 채팅 프리팹")]
        public CallConversation CallPrefab;

        private CallConversation callConversation;
        private PhoneControl phone;
        private IPhoneApplication app;

        // Start is called before the first frame update
        public override void OnNext(object value)
        {

        }

        public override void Play()
        {
            base.Play();
            callConversation = PhoneUtil.InstantiateUI(CallPrefab, out phone);
            callConversation.gameObject.SetActive(true);
            callConversation.ClearAction += Complete;
            callConversation.onIgnoreEvent += Ignore;
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);
            app.SetActiveBackground(true);
            phone.PhoneViewRotate(0);
            //CallObject.SettingDate()
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