using GamePlay.Event;
using Manager;
using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_Password : QuestBase
    {

        public PasswordToLine Password;
        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            var PasswordObj = UIManager.InstantiateFromPhone(Password);
            PasswordObj.gameObject.SetActive(true);
        }
    }
}