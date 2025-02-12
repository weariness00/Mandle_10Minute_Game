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
        public TextMeshProUGUI InputAccount;
        public TextMeshProUGUI InputAmount;
        public MMF_Player pre_sign;

        [Header("마지막 확인 텍스트")]
        public TextMeshProUGUI CheckText;
        public TextMeshProUGUI CheckAmountText;
        [Space]

        [Header("이체 해야 할 정보")]
        public string AnswerAccount;
        public int AnswerAmount;
        public string PassbookOwner;

        private int CurrentView = 1; //현재 화면

        [Header("은행 화면 정보")]
        public GameObject BankTransfer;
        public GameObject BankFinish;


        public Action ClearAction;
        public Action IgnoreAction;


        public void Init()
        {
            ChangeView(1);
            InputAccount.text = "";
            InputAmount.text = "";
            CheckText.text = "";
            CheckAmountText.text = "";
        }

        public void SettingData(string Name, string Account , int Amount)
        {
            AnswerAccount = Account;
            AnswerAmount = Amount;
            PassbookOwner = Name;
        }

        public void InputComplete() //입력 정보 확인 
        {
            string inputText = InputAccount.text.Trim();
            inputText = Regex.Replace(inputText, @"\u200B", "");
            string answerText = AnswerAccount.Trim().Replace("\n", "").Replace("\r", "");

            if (inputText == answerText)// 계좌가 올바르게 되었을 경우
            {
                CheckTextSet();
                ChangeView(2);
            }
            else // 계좌가 올바르지 않을 경우
            {
                pre_sign.PlayFeedbacks(); //임시 경고 표시
            }
        }
        private void CheckTextSet() // 입력 정보 확인 텍스트 수정
        {
            CheckText.text = "Account\n" + AnswerAccount + "\nBank\n" + PassbookOwner + ".\n";
            CheckAmountText.text = AddCommas(InputAmount.text) + "$";
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
        public void BankComplete() // 입력정보 최종 확인 후 송금
        {
            Debug.Log("송금 완료");
            string inputText = InputAmount.text.Trim();
            inputText = Regex.Replace(inputText, @"\u200B", "");
            int Amountdifference = 0;
            if (inputText != "")
                Amountdifference = AnswerAmount - int.Parse(inputText);


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
            if (index == 1)
            {
                BankTransfer.transform.DOLocalMoveX(0,1f);
                BankFinish.transform.DOLocalMoveX(450, 1f);
            }
            if (index == 2)
            {
                BankTransfer.transform.DOLocalMoveX(-450, 1f);
                BankFinish.transform.DOLocalMoveX(0, 1f);
            }
            CurrentView = index;
        }
    }
}