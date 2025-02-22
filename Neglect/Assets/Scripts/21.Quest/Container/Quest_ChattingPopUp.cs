using GamePlay.Phone;
using GamePlay.PopUp;

namespace Quest.Container
{
    public class Quest_ChattingPopUp : QuestBase
    {
        public PopUpPad popUpPrefab;

        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            var popUp = PhoneUtil.InstantiateUI(popUpPrefab);
            popUp.button.onClick.AddListener(Complete);
            popUp.destroyPopUpEvent.AddListener(Ignore);

            if(eventData.textArray.Length >= 1) popUp.titleText.text = eventData.textArray[0];
            if(eventData.textArray.Length >= 2) popUp.explainText.text = eventData.textArray[1];
            
            PhoneUtil.currentPhone.PhoneVibration();
        }

        public override void Complete()
        {
            base.Complete();
            var phone = PhoneUtil.currentPhone;
            phone.applicationControl.OpenApp("Chatting");
            phone.PhoneViewRotate(PhoneViewType.Vertical);
        }
    }
}

