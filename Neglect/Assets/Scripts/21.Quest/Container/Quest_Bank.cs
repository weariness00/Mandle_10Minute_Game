using GamePlay.Event;
using GamePlay.Phone;
using Manager;
using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace Quest.Container
{
    public class Quest_Bank : QuestBase
    {

        [Header("소환할 오브젝트")]
        public BankReadMemo Memo;
        public BankMoneyTransfer Bank;
        public Vector3 SpawnPos;

        public BankReadMemo BankMemo;
        public BankMoneyTransfer BankScreen;
        public override void OnNext(object value)
        {
        }

        public void Start()
        {
            
        }
        public override void Play()
        {
            base.Play();
            BankScreen = PhoneUtil.InstantiateUI(Bank , out var phone);
            BankScreen.gameObject.SetActive(true);

            BankMemo = Instantiate(Memo, SpawnPos, transform.rotation);
            BankMemo.gameObject.SetActive(true);

            BankMemo.TextSetting("owner", "123456", 50000); // 메모지에 표시 될 (사람 이름 _ 계좌 번호 _ 돈) 설정
            BankScreen.SettingData("owner", "123456", 50000); // 은행에서 이체해야 되는 (사람 이름 _ 계좌 번호 _ 돈) 설정

            BankScreen.ClearAction += Complete; // 성공시
            BankScreen.IgnoreAction += Ignore; // 실패시
            phone.PhoneViewRotate(0);
        }

        public override void Complete()
        {
            PrefabFinish();
            base.Complete();
        }

        public override void Ignore()
        {
            PrefabFinish();
            base.Ignore();
        }
        
        public void PrefabFinish()
        {
            BankMemo.HideAnimation(); 
            
            //BankScreen.
        }
        public void PrefabFinishCounter() //강제로 
        {

        }
    }
}