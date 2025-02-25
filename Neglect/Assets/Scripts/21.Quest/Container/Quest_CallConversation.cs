using GamePlay.Event;
using GamePlay.Phone;
using GamePlay.Talk;
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
            callConversation.ClearAction += isReverse ? Ignore : Complete;
            callConversation.onIgnoreEvent += isReverse ? Complete : Ignore;
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);
            app.SetActiveBackground(true);

            callConversation.SetTalkData(TalkingScriptableObject.Instance.GetTalkData(eventData.extraDataIDArray.Length == 0 ? -1 : eventData.extraDataIDArray[0]));
            if (eventData.textArray.Length > 0) callConversation.ChatName.text = eventData.textArray[0];

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