using GamePlay.PopUp;
using Manager;
using System;
using UnityEngine.Serialization;

namespace Quest.Container
{
    public class Quest_PopUp : QuestBase
    {
        public PopUpPad popUpPrefab;

        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            var popUp = PhoneUtil.InstantiateUI(popUpPrefab);
            popUp.completeButton.onClick.AddListener(Complete);
            popUp.ignoreButton.onClick.AddListener(Ignore);
            popUp.destroyPopUpEvent.AddListener(Ignore);
        }
    }
}

