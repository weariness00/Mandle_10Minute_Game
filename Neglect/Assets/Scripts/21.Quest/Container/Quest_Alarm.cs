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
            
            if (eventData.extraDataIDArray.Length > 1) isLoop = eventData.extraDataIDArray[1] == -45;
            
            alarm = PhoneUtil.InstantiateUI(questPrefab, out var phone_);
            phone = phone_;
            alarm.complete += Complete;
            alarm.ignoreEvent += () => StartCoroutine(IgnoreEnumerator());
            alarm.TimeSet("10:00");
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);

            phone.FadeOut(0.1f, Color.black);
            phone.PhoneViewRotate(PhoneViewType.Vertical, ()=> phone.FadeIn(1f, Color.black));
            
            PhoneUtil.currentPhone.PhoneVibration();
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

        private IEnumerator IgnoreEnumerator()
        {
            QuestManager.Instance.Remove(this);
            phone.applicationControl.OpenApp(app);
            var oneWait = new WaitForSeconds(1f);
            yield return new WaitForSeconds(10f);
            while (QuestManager.HasInstance && QuestManager.Instance.IsHasPlayQuest)
                yield return oneWait;
            if (QuestManager.HasInstance && QuestManager.Instance.isQuestStart)
            {
                Ignore();
            }
        }
    }
}