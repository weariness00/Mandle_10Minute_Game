using DG.Tweening;
using GamePlay;
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

        private Sequence vibrationSequence;
        public override void OnNext(object value)
        {
            
        }

        public override void Play()
        {
            base.Play();
            
            if (eventData.extraDataIDArray.Length > 1) isLoop = eventData.extraDataIDArray[1] == -45;
            
            alarm = PhoneUtil.InstantiateUI(questPrefab, out phone);
            alarm.complete += Complete;
            alarm.ignoreEvent += Ignore;

            int playTimeMinutes = Mathf.FloorToInt(GameManager.Instance.playTimer.Current)/60 + 50+ 11 *60; //현재 시각 분으로 환산 ( 11시 + 55분 + 플레이타임/60)
            int minutes = playTimeMinutes % 60;
            int hours = playTimeMinutes / 60;

            alarm.TimeSet(hours +  ":" + minutes);
            app = phone.applicationControl.currentPlayApplication;
            phone.applicationControl.PauseApp(app);

            phone.interfaceGroupOnOffButton.gameObject.SetActive(false);
            phone.FadeOut(0, Color.black);
            phone.PhoneViewRotate(PhoneViewType.Vertical, ()=> phone.FadeIn(1f, Color.black));

            vibrationSequence = phone.PhoneVibrationLoop(0.3f, 1f);
        }

        public override void Complete()
        {
            phone.applicationControl.OpenApp(app);
            vibrationSequence?.Kill();
            phone.FadeIn(0f, Color.black);
            base.Complete();
        }

        public override void Ignore()
        {
            vibrationSequence?.Kill();
            phone.applicationControl.OpenApp(app);
            phone.FadeIn(0f, Color.black);
            base.Ignore();
        }

        public override void Failed()
        {
            base.Failed();
            vibrationSequence?.Kill();
            phone.FadeIn(0f, Color.black);
            if(alarm) Destroy(alarm.gameObject);
        }
    }
}