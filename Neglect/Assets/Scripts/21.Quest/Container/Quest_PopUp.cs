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
            popUp.leftButton.onClick.AddListener(Complete);
            popUp.rightButton.onClick.AddListener(Ignore);
            popUp.destroyPopUpEvent.AddListener(Ignore);
        }
    }
}

