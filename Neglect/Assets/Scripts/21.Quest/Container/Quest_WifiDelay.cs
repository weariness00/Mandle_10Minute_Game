using GamePlay.Event;
using GamePlay.Phone;

namespace Quest.Container
{
    public class Quest_WifiDelay : QuestBase
    {
        public WiFiDelay WifiDelayPrefab;

        private WiFiDelay wiFiDelay;
        private PhoneControl phone;
        private IPhoneApplication app;

        public override void OnNext(object value)
        {

        }
        public override void Play()
        {
            base.Play();
            wiFiDelay = PhoneUtil.InstantiateUI(WifiDelayPrefab , out phone);
            wiFiDelay.Complete += Complete;

            app = phone.applicationControl.currentPlayApplication;
            if (eventData.extraDataIDArray.Length > 1) isLoop = eventData.extraDataIDArray[1] == -45;
            
            phone.applicationControl.PauseApp(app);
            app.SetActiveBackground(true);
            phone.PhoneViewRotate(1);
        }

        public override void Complete()
        {
            base.Complete();
            phone.applicationControl.OpenApp(app);
        }

        public override void Ignore()
        {
            base.Ignore();
            phone.applicationControl.OpenApp(app);
        }

        public override void Failed()
        {
            base.Failed();
            if(wiFiDelay) Destroy(wiFiDelay.gameObject);
        }
    }
}