using GamePlay.Phone;
using GamePlay.PopUp;
using System.Linq;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_ChattingPopUp : QuestBase
    {
        public PopUpPad popUpPrefab;

        [HideInInspector] public PopUpPad popUp;

        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            popUp = PhoneUtil.InstantiateUI(popUpPrefab);
            if(eventData.textArray.Length >= 1) popUp.titleText.text = eventData.textArray[0];
            if(eventData.textArray.Length >= 2) popUp.explainText.text = eventData.textArray[1];

            isReverse = eventData.extraDataIDArray.Contains(-99);
            isLoop = eventData.extraDataIDArray.Contains(-45);
            
            popUp.button.onClick.AddListener(Complete);
            popUp.destroyPopUpEvent.AddListener(Ignore);
            
            PhoneUtil.currentPhone.PhoneVibration();
        }

        public override void Complete()
        {
            base.Complete();
            var phone = PhoneUtil.currentPhone;
            phone.applicationControl.OpenApp("Chatting");
        }

        public override void Failed()
        {
            base.Failed();
            if(popUp) Destroy(popUp.gameObject);
        }
    }
}

