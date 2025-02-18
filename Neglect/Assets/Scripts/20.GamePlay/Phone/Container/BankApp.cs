using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using GamePlay.Event;
using UnityEditor;
using UnityEngine.Serialization;
using Util;

namespace GamePlay.Phone
{
    public partial class BankApp : MonoBehaviour
    {
        public bool isClearPassword; // 패스워드 통과 했는지
        [FormerlySerializedAs("Password")] public PasswordToLine password;

        public BankMemo BankMemo;
        
        public int RandomAmount;
        public string RandomAccount;
        public List<int> RandomPassword;

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
        
        [Header("결과 정보")]
        public Button resultOkButton;
        public TMP_Text resultText;
        
        [Header("은행 화면 정보")]
        public RectTransform bankAccountRectTransform;
        public RectTransform bankAmountRectTransform;
        public RectTransform bankFinishRectTransform;
        public RectTransform bankResultRectTransform;

        public Action completeAction;
        public Action ignoreAction;

        public void Awake()
        {
            password.ClearAction += PasswordClear;
        }

        // 패스워드 초기화
        public void InitPassword()
        {
            UniqueRandom intRandom = new(1, 9);
            RandomPassword.Clear();
            for (int i = 0; i < 4; i++)
                RandomPassword.Add(intRandom.RandomInt());
            password.SettingEvent(RandomPassword);
        }
        
        // 송금할 돈 초기화
        public void InitTransferMoney()
        {
            for (int i = 0; i < 8; i++)
            {
                if (i == 0)
                    RandomAccount += UnityEngine.Random.Range(1, 10).ToString();
                else
                    RandomAccount += UnityEngine.Random.Range(0, 10).ToString();
            }
            for (int i = 0; i < 6; i++)
            {
                if( i >= 3)
                    RandomAmount = RandomAmount * 10 + 0;
                else if (i == 0)
                    RandomAmount = RandomAmount * 10 + UnityEngine.Random.Range(1, 10);
                else
                    RandomAmount = RandomAmount * 10 + UnityEngine.Random.Range(0, 10);
            }
            AnswerAccount = RandomAccount;
            AnswerAmount = RandomAmount;
            
            BankMemo.TextSetting("To Owner", RandomAccount, RandomAmount);
        }

        public bool IsSkip() // 비정상적인 패스워드 입력 체크
        {
            int BackIndex = RandomPassword.Count - 1;
            int A = RandomPassword[BackIndex]-1;
            int B = RandomPassword[BackIndex - 1]-1;
            int[,] C = new int[,] { { 0, 2 }, { 3, 5 }, { 6, 8 }, { 0, 8 }, { 2, 6 }, { 0, 6 }, { 1, 7 }, { 2, 8 } };
            for (int i = 0; i < 8; i++)
            {
                if (A == C[i, 0] && B == C[i, 1] || B == C[i, 0] && A == C[i, 1])
                    return true;
            }
            return false;
        }
        
        public void SettingData(string Name, string Account, int Amount)
        {
            AnswerAccount = Account;
            AnswerAmount = Amount;
            PassbookOwner = Name;
        }

        public void SetAccount() //입력 정보 확인 
        {
            CheckText.text = AddBar(AnswerAccount) + "\n" + PassbookOwner + "님에게\n";

            string pre1 = InputAmount.ToString();
            CheckAmountText.text = AddCommas(pre1) +"원";
        }
        public void CheckAccount() // 입력 정보 확인 텍스트 수정
        {
            if (AnswerAccount == InputAccount)
            {
                ChangeView(2);
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
            ChangeView(1);
        }
        public void KeyPadMove(bool p)
        {
            if (KeyPad_Image.Count == 0)
            {
                for (int i = 0; i < KeyPad_objects.Count; i++)
                    KeyPad_Image.Add(KeyPad_objects[i].GetComponent<Image>());
            }
            if (CurrentView == 3)
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

            ChangeView(4);
            if (Amountdifference == 0)
            {
                resultText.text = $"{PassbookOwner}님에게\n{InputAmount}을 송금했습니다.";
                completeAction?.Invoke();
            }
            else
            {
                ignoreAction?.Invoke();
            }

        }
        public void ChangeView(int index) // 임시 화면 전환
        {
            void Move(int startIndex)
            {
                password.transform.DOLocalMoveX(600 * startIndex++, 1f);
                bankAccountRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankAmountRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankFinishRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankResultRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
            }

            Move(-index);
            if (index == 3)
            {
                SetAccount();
                KeyPadMove(false);
            }
            CurrentView = index;
        }

        public void SetText()
        {
            string pre = InputAccount;
            string pre1 = InputAmount.ToString();
            InputAccountText.text = AddBar(pre);
            InputAmountText.text = AddCommas(pre1) +"원";
        }
        public void InputClickButton(int num)
        {
            if (CurrentView == 1) //계좌 입력
            {
                if (num >= 0 && num <= 9)
                {
                    if (InputAccount.Length > 9)
                        return;
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
            else if (CurrentView == 2) //숫자 입력
            {
                if (num >= 0 && num <= 9)
                {

                    if (InputAmount >= 9999999)
                        return;
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
        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;
            mainCanvas.worldCamera = phone.phoneCamera;
            password.phone = phone;
            
            BankMemo.gameObject.layer = 0;
            BankMemo.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(false);
            
            resultOkButton.onClick.AddListener(() =>
            {
                _phone.applicationControl.OnHome();
            });
        }

        public void AppPause(PhoneControl phone)
        {

        }

        public void AppPlay(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(true);
            password.phone = phone;
            
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

#if UNITY_EDITOR
    [CustomEditor(typeof(BankApp))]
    public class BankAppEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as BankApp;
            
            if(!EditorApplication.isPlaying) return;
            if (GUILayout.Button("패스워드 초기화"))
            {
                script.InitPassword();
            }

            if (GUILayout.Button("송금 초기화"))
            {
                script.InitTransferMoney();
                script.BankMemo.ShowAnimation();
            }
        }
    }
#endif
}