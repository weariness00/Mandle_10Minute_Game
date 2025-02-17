using GamePlay.Phone;

namespace Quest.Container
{
    public class Quest_Bank : QuestBase
    {
        private BankApp bankApp;

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
            bankApp = phoen.applicationControl.GetApp("Bank") as BankApp;
            bankApp.InitPassword();
            bankApp.InitTransferMoney();
            bankApp.completeAction += Complete;
            bankApp.ignoreAction += Ignore;
        }
    }
}