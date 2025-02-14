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

        public void Start() 
        {

        }

        public override void Play()
        {
            base.Play();
            var PasswordObj = PhoneUtil.InstantiateUI(Password, out var phone);
            PasswordObj.gameObject.SetActive(true);
            PasswordObj.SettingEvent("Hint : 1 2 3 4 5 6 7 8 9" , "[1,2,3,4,5,6,7,8,9]");
            PasswordObj.ClearAction += Complete;
            PasswordObj.phone = phone;
        }


        public override void Complete()
        {
            Debug.Log("성공");
            base.Complete();
        }
        public override void Ignore()
        {
            base.Ignore();
        }
    }
}