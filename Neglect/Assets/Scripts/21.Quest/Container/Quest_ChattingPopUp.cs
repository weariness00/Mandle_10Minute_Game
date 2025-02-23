using GamePlay.Phone;
using GamePlay.PopUp;

namespace Quest.Container
{
    public class Quest_ChattingPopUp : QuestBase
    {
        public PopUpPad popUpPrefab;

        private PopUpPad popUp;

        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            popUp = PhoneUtil.InstantiateUI(popUpPrefab);
            if(eventData.textArray.Length >= 1) popUp.titleText.text = eventData.textArray[0];
            if(eventData.textArray.Length >= 2) popUp.explainText.text = eventData.textArray[1];

            if (eventData.extraDataIDArray.Length >= 1) isReverse = eventData.extraDataIDArray[0] == -99;
            
            popUp.button.onClick.AddListener(isReverse ? Ignore : Complete);
            popUp.destroyPopUpEvent.AddListener(isReverse ? Complete : Ignore);
            
            PhoneUtil.currentPhone.PhoneVibration();
        }

        public override void Complete()
        {
            base.Complete();
            var phone = PhoneUtil.currentPhone;
            phone.applicationControl.OpenApp("Chatting");
            phone.PhoneViewRotate(PhoneViewType.Vertical);
        }

        public override void Failed()
        {
            base.Failed();
            if(popUp) Destroy(popUp.gameObject);
        }
    }
}

