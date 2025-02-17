using DG.Tweening;
using GamePlay.Phone;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using GamePlay.Event;
using Manager;

namespace GamePlay.Phone
{
    public partial class BankApp : MonoBehaviour
    {
        public int RandomAmount;
        public string RandomAccount;
        public string RandomPassword;

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

        public List<GameObject> KeyPad_objects = new();
        public List<Image> KeyPad_Image = new();
        public List<TextMeshProUGUI> KeyPad_Text = new();

        [Space]
        [Header("이체 해야 할 정보")]
        public string AnswerAccount;
        public int AnswerAmount;
        public string PassbookOwner;

        private bool IsKeyPad = false;
        private int CurrentView = -1; //현재 화면

        [Header("은행 화면 정보")]
        public GameObject BankAccount;
        public GameObject BankAmount;
        public GameObject BankFinish;

        public Action ClearAction;
        public Action IgnoreAction;
        public Action HideComplete;

        public void lotto()
        {
            List<int> RandomNum = new List<int> { 1, 2 ,3,4,5,6,7,8,9};
            for (int i = 0; i < 4; i++)
            {
                int pre = UnityEngine.Random.Range(0, RandomNum.Count);
                RandomPassword += RandomNum[pre].ToString();
                RandomNum.RemoveAt(pre);
            }
            for (int i = 0; i < 8; i++)
            {
                if (i == 0)
                    RandomAccount += UnityEngine.Random.Range(1, 10).ToString();
                else
                    RandomAccount += UnityEngine.Random.Range(0, 10).ToString();
            }
            for (int i = 0; i < 6; i++)
            {
                if( i <= 3)
                    RandomAmount += RandomAmount * 10 + 0;
                else if (i == 0)
                    RandomAmount += RandomAmount * 10 + UnityEngine.Random.Range(1, 10);
                else
                    RandomAmount += RandomAmount * 10 + UnityEngine.Random.Range(0, 10);
            }
        }
        public void Init()
        {
            ChangeView(0);
            CheckText.text = "";
            CheckAmountText.text = "";
        }

        public void SettingData(string Name, string Account, int Amount)
        {
            AnswerAccount = Account;
            AnswerAmount = Amount;
            PassbookOwner = Name;
        }

        public void SetAccount() //입력 정보 확인 
        {

            CheckText.text = AddBar(AnswerAccount) + "\n" + PassbookOwner + "\n";

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
        public void PasswordClear()
        {
            ChangeView(0);
        }
        public void KeyPadMove(bool p)
        {
            if (KeyPad_Image.Count == 0)
            {
                for (int i = 0; i < KeyPad_objects.Count; i++)
                    KeyPad_Image.Add(KeyPad_objects[i].GetComponent<Image>());
            }
            if (CurrentView == 2)
                return;
            if (p == true && !IsKeyPad)
            {
                IsKeyPad = true;
                KeyPad.DOLocalMoveY(400, 0.5f).SetRelative(true);
                for (int i = 0; i < KeyPad_Image.Count; i++)
                {
                    KeyPad_Image[i].DOFade(0, 0f);
                    KeyPad_Image[i].DOFade(1, 0.5f);
                }
                for (int i = 0; i < KeyPad_Text.Count; i++)
                {
                    KeyPad_Text[i].DOFade(0, 0f);
                    KeyPad_Text[i].DOFade(1, 0.5f);
                }
            }
            else if (p == false && IsKeyPad)
            {
                KeyPad.DOLocalMoveY(-400, 0.5f).SetRelative(true);
                IsKeyPad = false;
                for (int i = 0; i < KeyPad_Image.Count; i++)
                {
                    KeyPad_Image[i].DOFade(0, 0.5f);
                }
                for (int i = 0; i < KeyPad_Text.Count; i++)
                {
                    KeyPad_Text[i].DOFade(1, 0f);
                    KeyPad_Text[i].DOFade(0, 0.5f);
                }
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
                if ((count == length / 2 - 1 || count == length / 2 + 1) && i != length - 1)
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
            if (index == -1 && CurrentView == 0)
            {

                Password.transform.DOLocalMoveX(0, 1f);
                BankAccount.transform.DOLocalMoveX(600, 1f);
                BankAmount.transform.DOLocalMoveX(1200, 1f);
                BankFinish.transform.DOLocalMoveX(1800, 1f);
            }
            if (index == 0 && (CurrentView == 1 || CurrentView == -1) )
            {
                Password.transform.DOLocalMoveX(-600, 1f);
                BankAccount.transform.DOLocalMoveX(0, 1f);
                BankAmount.transform.DOLocalMoveX(600, 1f);
                BankFinish.transform.DOLocalMoveX(1200, 1f);
            }
            if (index == 1)
            {
                Password.transform.DOLocalMoveX(-1200, 1f);
                BankAccount.transform.DOLocalMoveX(-600, 1f);
                BankAmount.transform.DOLocalMoveX(0, 1f);
                BankFinish.transform.DOLocalMoveX(600, 1f);
            }
            if (index == 2 && CurrentView == 1)
            {
                SetAccount();
                Password.transform.DOLocalMoveX(-1800, 1f);
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
                if (num >= 0 && num <= 9)
                {
                    InputAccount += num.ToString();
                }
                if (num == 10)
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
                    InputAmount = InputAmount * 10 + num;
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
        public partial class BankApp : IPhoneApplication
    {
        public Canvas mainCanvas;

        [Header("Phone 관련")]
        [SerializeField] private string appName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int verticalResolution;
        [SerializeField] private PhoneControl _phone;
        public string AppName { get => appName; }
        public Sprite AppIcon { get => icon; set => icon = value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public PhoneControl Phone => _phone;

        public bool isClearPassword; // 패스워드 통과 했는지
        public PasswordToLine Password;

        public BankReadMemo BankMemo;
        
        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;
            mainCanvas.worldCamera = phone.phoneCamera;
            Password.phone = phone;
            Password.ClearAction += PasswordClear;
            lotto(); // 패스워드 계좌번호 금액 랜덤 결정

            Password.SettingEvent("", "[1,2,3,6]");
            
            if(BankMemo == null)
            {
                BankMemo = Instantiate(BankMemo , new Vector3(5.5f,0,0), Quaternion.identity);
            }
            BankMemo.TextSetting("To Owner", RandomAccount, RandomAmount, "1 2 3 6");

            BankMemo.gameObject.layer = 0;
            
            BankMemo.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(false);
        }

        public void AppPause(PhoneControl phone)
        {

        }

        public void AppPlay(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(true);
            Password.phone = phone;
            
            BankMemo.ShowAnimation();
        }

        public void AppResume(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(false);

            BankMemo.ShowAnimation();
        }
        
        public void AppExit(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(false);
            BankMemo.HideAnimation();
        }

        public void AppUnInstall(PhoneControl phone)
        {

        }
    }
}