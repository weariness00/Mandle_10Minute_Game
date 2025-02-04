using GamePlay.PopUp;
using Manager;
using System;

namespace Quest.Container
{
    public class Quest_Calling : QuestBase
    {
        public PopUpPad callingPopUpPrefab;
        
        public override void OnCompleted()
        {
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
            var popUp = UIManager.InstantiateFromPhone(callingPopUpPrefab);
        }
    }
}

