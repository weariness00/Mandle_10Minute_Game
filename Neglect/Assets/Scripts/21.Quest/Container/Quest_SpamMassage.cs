using GamePlay.PopUp;
using Manager;
using System;

namespace Quest.Container
{
    public class Quest_SpamMassage : QuestBase
    {
        public PopUpPad spamPopUp;
        
        public override void OnCompleted()
        {
            state = QuestState.Completed;
        }

        public override void OnError(Exception error)
        {
        }

        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            var spawnSpam = UIManager.InstantiateFromPhone(spamPopUp);
            spawnSpam.destroyPopUpEvent.AddListener(OnCompleted);
        }
    }
}

