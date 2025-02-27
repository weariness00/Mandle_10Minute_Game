using GamePlay.App;
using GamePlay.Phone;

namespace Quest.Container
{
    public class Quest_Bank : QuestBase
    {
        private BankApp bankApp;

        private AppButton appButton;

        public override void OnNext(object value)
        {
        }

        public void Start()
        {
            
        }
        public override void Play()
        {
            base.Play();
            var phoen = PhoneUtil.currentPhone;
            
            foreach (AppButton button in phoen.applicationControl.GetHomeApp().GetAllAppButton())
            {
                button.button.interactable = false;
            }
            bankApp = phoen.applicationControl.GetApp("Bank") as BankApp;
            appButton = phoen.applicationControl.GetHomeApp().GetAppButton(bankApp);
            appButton.button.interactable = true;
            bankApp.InitPassword();
            bankApp.InitTransferMoney();
            bankApp.eventData = eventData;
            bankApp.completeAction += Complete;
            bankApp.ignoreAction += Ignore;
        }
        
        public override void Failed()
        {
            base.Failed();
            if(appButton) appButton.button.interactable = false;
        }
    }
}