using GamePlay.Event;
using GamePlay.Phone;
using Manager;
using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
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

        public int RandomAmount;
        public string RandomAccount;

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

            lotto();
            BankMemo.TextSetting("owner", RandomAccount, RandomAmount); // 메모지에 표시 될 (사람 이름 _ 계좌 번호 _ 돈) 설정
            BankScreen.SettingData("owner", RandomAccount, RandomAmount); // 은행에서 이체해야 되는 (사람 이름 _ 계좌 번호 _ 돈) 설정

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
        
        public void lotto()
        {
            for(int i = 0; i < 8; i++)
            {
                if (i == 0)
                    RandomAccount += UnityEngine.Random.Range(1, 9).ToString();
                else
                    RandomAccount += UnityEngine.Random.Range(0, 9).ToString();
            }
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                    RandomAmount += UnityEngine.Random.Range(1, 9);
                else
                    RandomAmount += UnityEngine.Random.Range(0, 9);
            }
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