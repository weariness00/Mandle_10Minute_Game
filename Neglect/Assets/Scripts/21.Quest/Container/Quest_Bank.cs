using GamePlay;
using GamePlay.App;
using GamePlay.Narration;
using GamePlay.Phone;

namespace Quest.Container
{
    public class Quest_Bank : QuestBase
    {
        private BankApp bankApp;
        
        private PhoneControl phone;
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
            phone = PhoneUtil.currentPhone;
            
            foreach (AppButton button in phone.applicationControl.GetHomeApp().GetAllAppButton())
                button.button.interactable = false;
            bankApp = phone.applicationControl.GetApp("Bank") as BankApp;
            appButton = phone.applicationControl.GetHomeApp().GetAppButton(bankApp);
            appButton.button.interactable = true;
            bankApp.InitPassword();
            if (bankApp.eventData != eventData)
            {
                // 마지막 확인 누를때 돈 잘 보내면 홈으로 가기
                bankApp.resultOkButton.onClick.AddListener(() =>
                {
                    if (bankApp.Amountdifference == 0)
                        phone.applicationControl.OnHome();
                });
                bankApp.InitTransferMoney();
                bankApp.eventData = eventData;
                bankApp.completeAction += Complete;
                bankApp.ignoreAction += Ignore;
            }
            else if (bankApp.Amountdifference < 0)
            {
                bankApp.bankTransactionOnButton.onClick.AddListener(Complete);
            }
        }

        public override void Complete()
        {
            if (bankApp.Amountdifference == 0)
            {
                subscription?.Dispose();
                state = isReverse ? QuestState.Failed : QuestState.Completed;
                if (rootQuest != null) rootQuest.state = state;
                if (eventData.completeNarrationID != -1) NarrationManager.Instance.StartNarrationID(eventData.completeNarrationID);
                appButton.button.interactable = false;
                onCompleteEvent?.Invoke(null);
                
                QuestManager.Instance.Remove(this);
                bankApp.eventData = null;
                GameManager.Instance.GameClear();
            }
            else if (bankApp.Amountdifference < 0)
            {
                bankApp.bankTransactionOnButton.onClick.AddListener(() =>
                {
                    var home = phone.applicationControl.GetHomeApp();
                    home.InterfaceActive(true);
                    appButton.button.interactable = false;
                    bankApp.Amountdifference = 0;
                });
                bankApp.AnswerAccount = "";
                bankApp.BankMemo.memoText.text = "";
                base.Complete();
                phone.applicationControl.OnHome();
            }
        }

        public override void Ignore()
        {
            base.Ignore();
            phone.applicationControl.OnHome();
        }

        public override void Failed()
        {
            base.Failed();
            if (appButton) appButton.button.interactable = false;
            if (phone.applicationControl.currentPlayApplication == bankApp as IPhoneApplication)
                phone.applicationControl.OnHome();
        }
    }
}