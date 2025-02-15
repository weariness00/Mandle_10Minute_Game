using DG.Tweening;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace GamePlay.Event
{
    public class BankMoneyTransfer : MonoBehaviour
    {
        // Start is called before the first frame update

        [Header("패스워드 완료 후 계좌 이체 텍스트")]
        public TextMeshProUGUI InputAmountText;
        public TextMeshProUGUI InputAccountText;
        public int InputAmount;   //입력된 통장 번호
        public string InputAccount;  //입력된 계좌 번호
        public MMF_Player pre_sign;  // 계좌가 없을때 뜨는 싸인

        public RectTransform KeyPad;



        [Header("마지막 확인 텍스트")]
        public TextMeshProUGUI CheckText;
        public TextMeshProUGUI CheckAmountText;
        


        
        [Space]
        [Header("이체 해야 할 정보")]
        public string AnswerAccount;
        public int AnswerAmount;
        public string PassbookOwner;

        private bool IsKeyPad = false;
        private int CurrentView = 0; //현재 화면

        [Header("은행 화면 정보")]
        public GameObject BankAccount;
        public GameObject BankAmount;
        public GameObject BankFinish;


        public Action ClearAction;
        public Action IgnoreAction;
        public Action HideComplete;


        public void Init()
        {
            ChangeView(0);
            CheckText.text = "";
            CheckAmountText.text = "";
        }

        public void SettingData(string Name, string Account , int Amount)
        {
            AnswerAccount = Account;
            AnswerAmount = Amount;
            PassbookOwner = Name;
        }

        public void SetAccount() //입력 정보 확인 
        {
            
            CheckText.text = AddBar(AnswerAccount) +"\n" + PassbookOwner+"\n";

            string pre1 = InputAmount.ToString();
            CheckAmountText.text = AddCommas(pre1);
        }
        public void CheckAccount() // 입력 정보 확인 텍스트 수정
        {
            if (AnswerAccount == InputAccount)
            {
                ChangeView(1);
            }
            else
            {
                pre_sign.PlayFeedbacks();
            }
        }
        public static string AddCommas(string input)
        {
            input = input.Trim();
            input = Regex.Replace(input, @"\u200B", "");
            int length = input.Length;
            if (length <= 3)
                return input;
            string result = "";
            int count = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                result = input[i] + result;
                count++;

                if (count % 3 == 0 && i != 0)
                {
                    result = "," + result;
                }
            }
            return result;
        }
        public void KeyPadMove(bool p)
        {
            if (CurrentView == 2)
                return;
            if (p == true&&!IsKeyPad)
            {
                IsKeyPad = true;
                KeyPad.DOMoveY(400, 0.5f).SetRelative(true);
            }
            else if (p == false &&IsKeyPad)
            {
                KeyPad.DOMoveY(-400, 0.5f).SetRelative(true);
                IsKeyPad = false;
            }
        }
        public static string AddBar(string input)
        {
            input = input.Trim();
            int length = input.Length;
            if (length <= 4)
                return input;
            string result = "";

            int count = 0;
            for (int i = 0; i < length; i++)
            {
                result += input[i];
                count++;
                if ((count == length/2 -1 || count == length / 2 + 1) && i != length-1)
                {
                    result += "-";
                }
            }

            return result;
        }
        public void BankComplete() // 입력정보 최종 확인 후 송금
        {
            int Amountdifference = 0;
            Amountdifference = AnswerAmount - InputAmount;
            Debug.Log(Amountdifference + "만큼 금액 차이 발생"); //  이 금액이 후속이벤트 
            if (Amountdifference == 0)
            {
                ClearAction();
                Destroy(gameObject); //사라지는 애니메이션 일단은 삭제
            }
            else
            {
                IgnoreAction();
                Destroy(gameObject); //사라지는 애니메이션 일단은 삭제
            }
            
        }
        public void ChangeView(int index) // 임시 화면 전환
        {
            if (index == 0 && CurrentView==1)
            {
                BankAccount.transform.DOLocalMoveX(0,1f);
                BankAmount.transform.DOLocalMoveX(600, 1f);
                BankFinish.transform.DOLocalMoveX(1200, 1f);
            }
            if (index == 1)
            {
                BankAccount.transform.DOLocalMoveX(-600, 1f);
                BankAmount.transform.DOLocalMoveX(0, 1f);
                BankFinish.transform.DOLocalMoveX(600, 1f);
            }
            if (index == 2&& CurrentView == 1)
            {
                SetAccount();
                BankAccount.transform.DOLocalMoveX(-1200, 1f);
                BankAmount.transform.DOLocalMoveX(-600, 1f);
                BankFinish.transform.DOLocalMoveX(0, 1f);
                KeyPadMove(false);
            }
            CurrentView = index;
        }

        public void HideAnimation()
        {
            
        }

        public void SetText()
        {
            string pre = InputAccount;
            string pre1 = InputAmount.ToString();
            InputAccountText.text = AddBar(pre);
            InputAmountText.text = AddCommas(pre1);
        }
        public void InputClickButton(int num)
        {
            if (CurrentView == 0) //계좌 입력
            {
                if(num >=0 && num <= 9)
                {
                    InputAccount += num.ToString();
                }
                if(num == 10)
                {
                    if (!string.IsNullOrEmpty(InputAccount))
                    {
                        InputAccount = InputAccount.Remove(InputAccount.Length - 1);
                    }
                }
                if (num == 11)
                {

                    KeyPadMove(false);
                }
            }
            else if (CurrentView == 1) //숫자 입력
            {
                if (num >= 0 && num <= 9)
                {
                    InputAmount = InputAmount*10+ num;
                }
                if (num == 10)
                {
                    InputAmount = InputAmount / 10;
                }
                if (num == 11)
                {
                    KeyPadMove(false);
                }
            }
            SetText();

        }
    }
}