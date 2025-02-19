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

        [Header("생성된 채팅 오브젝트")]
        public CallConversation CallObject;

        private PhoneControl phone;
        private IPhoneApplication app;

        // Start is called before the first frame update
        public override void OnNext(object value)
        {

        }

        public override void Play()
        {
            base.Play();
            CallObject = PhoneUtil.InstantiateUI(CallPrefab, out phone);
            CallObject.gameObject.SetActive(true);
            CallObject.ClearAction += Complete;
            app = phone.applicationControl.currentPlayApplication;
            app.AppPause(phone);
            phone.PhoneViewRotate(0);
            //CallObject.SettingDate()
        }

        public override void Complete()
        {
            base.Complete();
            app.AppResume(phone);
        }

        public override void Ignore()
        {
            base.Ignore();
        }
    }
}