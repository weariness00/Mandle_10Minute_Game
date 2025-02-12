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

        public void Start() // 테스트 
        {
            Password.SettingEvent("Hint : 1 2 3 4 5 6 7 8 9", "[1,2,3,4,5,6,7,8,9]");
            Password.ClearAction += Complete;

        }

        public override void Play()
        {
            base.Play();
            var PasswordObj = UIManager.InstantiateFromPhone(Password);
            PasswordObj.gameObject.SetActive(true);
            //PasswordObj.SettingEvent("Hint : 1 2 3 6" , "[1,2,3,4,5,6,7,8,9]");

           
        }


        public override void Complete()
        {
            Debug.Log("잠금해제");
           // base.Complete();

        }
        public override void Ignore()
        {
            base.Ignore();
        }
    }
}