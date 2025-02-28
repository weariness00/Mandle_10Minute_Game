using GamePlay.Event;
using GamePlay.Phone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace Quest.Container
{
    public class Quest_CallingScreen : QuestBase
    {
        public CallingScreen phoneCallScreenPrefab;

        private CallingScreen callingScreen;
        private PhoneControl phone;
        private IPhoneApplication app;

        private Sequence vibrationSequence;
        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            callingScreen = PhoneUtil.InstantiateUI(phoneCallScreenPrefab, out phone);
            callingScreen.phone = phone;
            app = phone.applicationControl.currentPlayApplication;
            phone.interfaceGroupOnOffButton.gameObject.SetActive(false);
            phone.applicationControl.PauseApp(app);
            if (phone.viewType == PhoneViewType.Horizon)
            {
                phone.FadeOut(0f, Color.black);
                phone.PhoneViewRotate(PhoneViewType.Vertical, () => phone.FadeIn(1f, Color.black));
            }

            app.SetActiveBackground(true);
            
            callingScreen.name.text = eventData.textArray.Length == 0 ? "mom" : eventData.textArray[0];

            if (eventData.extraDataIDArray.Length >= 1) isReverse = eventData.extraDataIDArray[0] == -99;
            if (eventData.extraDataIDArray.Length >= 2) isLoop = eventData.extraDataIDArray[1] == -45;
            callingScreen.ClearAction += Complete;
            callingScreen.IgnoreAction += Ignore;


            vibrationSequence = phone.PhoneVibrationLoop(0.3f, 1f);
        }
        public override void Ignore()
        {
            vibrationSequence?.Kill();
            phone.applicationControl.OpenApp(app);
            base.Ignore();
        }

        public override void Failed()
        {
            vibrationSequence?.Kill();
            base.Failed();
            if(callingScreen) Destroy(callingScreen.gameObject);
        }

        public override void Complete()
        {
            vibrationSequence?.Kill();
            base.Complete();
        }
    }
}