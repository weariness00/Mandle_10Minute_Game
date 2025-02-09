using GamePlay.PopUp;
using Manager;
using System;
using UnityEngine.Serialization;

namespace Quest.Container
{
    public class Quest_PopUp : QuestBase
    {
        public PopUpPad popUp;

        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            var spawnSpam = UIManager.InstantiateFromPhone(popUp);
            spawnSpam.gameObject.SetActive(true);
        }
    }
}

