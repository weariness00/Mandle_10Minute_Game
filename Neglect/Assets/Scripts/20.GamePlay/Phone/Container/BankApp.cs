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
using GamePlay.Container;
using Quest;

namespace GamePlay.Phone
{
    public partial class BankApp : MonoBehaviour
    {
        public bool isClearPassword; // 패스워드 통과 했는지
        [FormerlySerializedAs("Password")] public PasswordToLine password;

        public BankMemo BankMemo;

        [HideInInspector] public EventData eventData;

        public int RandomAmount;
        public string RandomAccount;
        public List<int> RandomPassword;
        public int inputAmountsum;

        public CanvasGroup keyPadCanvasGroup;
        [Header("패스워드 완료 후 계좌 이체 텍스트")]
        public TextMeshProUGUI InputAmountText;
        public TextMeshProUGUI InputAccountText;
        public int InputAmount;   //입력된 통장 번호
        public string InputAccount;  //입력된 계좌 번호
        public MMF_Player pre_sign;  // 계좌가 없을때 뜨는 싸인
        public MMF_Player pre_sign_Current_Cash;  // 소지 금액보다 많은 금액을 쓸때 뜨는 싸인
        public RectTransform KeyPad;
        [Header("마지막 확인 텍스트")]
        public TextMeshProUGUI CheckText;
        public TextMeshProUGUI CheckAmountText;

        public List<GameObject> KeyPad_objects = new();
        public List<TextMeshProUGUI> KeyPad_Text = new();

        [Space]
        [Header("이체 해야 할 정보")]
        public string AnswerAccount;
        public int AnswerAmount;
        [HideInInspector] public int Amountdifference;
        public string PassbookOwner;
        private bool IsKeyPad = false;
        public int CurrentView = -1; //현재 화면
        [Header("이체할 대상")]
        public TMP_Text targetPassbookOwnerText;
        [Header("소지금액")]
        public int currentCash;
        public TMP_Text currentCashText;
        [Header("결과 정보")]
        public Button resultOkButton;
        public TMP_Text resultText;

        [Header("이체 은행 화면 정보")]
        public RectTransform bankMenuRectTransform;
        public RectTransform bankAccountRectTransform;
        public RectTransform bankAmountRectTransform;
        public RectTransform bankFinishRectTransform;
        public RectTransform bankResultRectTransform;

        [Header("거래내역 확인 은행 화면 정보")] 
        public Button bankTransactionOnButton; // 거래내역 확인 정보 키는 버튼
        public RectTransform bankTransactionTransform;

        public Action completeAction;
        public Action ignoreAction;

        public void Awake()
        {
            password.completeAction += PasswordClear;
            password.Init();
            
            TradeHistoryInit();//거래 내역 초기화
            InitCurrentCash();
        }

        // 패스워드 초기화
        public void InitPassword()
        {
            password.Init();

            UniqueRandom intRandom = new(1, 9);
            RandomPassword.Clear();
            for (int i = 0; i < 4; i++)
                RandomPassword.Add(intRandom.RandomInt());
            password.SetPassword(RandomPassword);
        }

        // 송금할 돈 초기화
        public void InitCurrentCash()
        {
            for (int i = 0; i < 7; i++)
            {
                if (i >= 4)
                    currentCash = currentCash * 10 + 0;
                else if (i == 0)
                    currentCash = currentCash * 10 + UnityEngine.Random.Range(1, 10);
                else
                    currentCash = currentCash * 10 + UnityEngine.Random.Range(0, 10);
            }
            currentCashText.text = AddCommas(currentCash.ToString());
        }

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
                if (i >= 3)
                    RandomAmount = RandomAmount * 10 + 0;
                else if (i == 0)
                    RandomAmount = RandomAmount * 10 + UnityEngine.Random.Range(1, 10);
                else
                    RandomAmount = RandomAmount * 10 + UnityEngine.Random.Range(0, 10);
            }
            
            AnswerAccount = RandomAccount;
            AnswerAmount = RandomAmount;

            BankMemo.TextSetting("To Owner", RandomAccount, RandomAmount);
            targetPassbookOwnerText.text = PassbookOwner +"님에게";
        }

        public bool IsSkip() // 비정상적인 패스워드 입력 체크
        {
            int BackIndex = RandomPassword.Count - 1;
            int A = RandomPassword[BackIndex] - 1;
            int B = RandomPassword[BackIndex - 1] - 1;
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
            string pre1Comma = AddCommas(pre1);
            CheckAmountText.text = pre1Comma + "원을 \n송금하겠습니까?";
            
            resultText.text = $"{PassbookOwner}님에게\n{pre1Comma}원을\n송금했습니다.";
        }
        public void CheckAccount() // 입력 정보 확인 텍스트 수정
        {
            if (AnswerAccount == InputAccount && AnswerAccount.Length != 0)
            {
                ChangeView(3);
            }
            else
            {
                pre_sign.PlayFeedbacks();
            }
        }
        public void CheckAmount() // 
        {
            if (currentCash >= InputAmount)
            {
                ChangeView(4);
            }
            else
            {
                pre_sign_Current_Cash.PlayFeedbacks();
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

            if (CurrentView != 2 && CurrentView != 3)
                return;
            if (p == true && !IsKeyPad)
            {
                KeyPadSuffle();

                IsKeyPad = true;
                KeyPad.DOLocalMoveY(400, 0.5f).SetRelative(true);

                keyPadCanvasGroup.alpha = 0;
                keyPadCanvasGroup.DOFade(1, 0.5f);

            }
            else if (p == false && IsKeyPad)
            {
                KeyPad.DOLocalMoveY(-400, 0.5f).SetRelative(true);
                IsKeyPad = false;

                keyPadCanvasGroup.alpha = 1;
                keyPadCanvasGroup.DOFade(0, 0.5f);
                
            }
        }
        public void KeyPadSuffle()
        {
            Transform ButtonDel = KeyPad.GetChild(3); // Del 버튼
            Transform ButtonEnd = KeyPad.GetChild(11); // End 버튼

            for (int i = 0; i < KeyPad.childCount; i++)
            {
                int num = UnityEngine.Random.Range(0, KeyPad.childCount);
                KeyPad.GetChild(i).SetSiblingIndex(num);
            }// 셔플


            ButtonDel.SetSiblingIndex(3); // 원상 복귀
            ButtonEnd.SetSiblingIndex(11); // 원상복귀
            if(ButtonDel.GetSiblingIndex() != 3)
                ButtonDel.SetSiblingIndex(3); // 원상 복귀
        }

        public static string AddBar(string input)
        {
            var result = input;

            int i = 0;
            if (input.Length >= 3) result = result.Insert(2 + i++, "-");
            if (input.Length >= 7) result = result.Insert(6 + i++, "-");

            return result;
        }

        public void BankComplete() // 입력정보 최종 확인 후 송금 사실상 마지막장면에는 뒤로가기 버튼이 없으므로 확정임 
        {
            Amountdifference = AnswerAmount - InputAmount;

            HistoryUpload(2, InputAmount); //송금한 금액 거래내역에 넣기
            currentCash -= InputAmount;
            currentCashText.text = AddCommas(currentCash.ToString()); //소지금액 차감 후 갱신
            if (Amountdifference <= 0)
            {
                HistoryUpload(1, -Amountdifference); //초과금 거래내역에 넣기
                currentCash += Amountdifference;
                currentCashText.text = AddCommas(currentCash.ToString()); //환불받은 초과금 추가 후 갱신
                completeAction?.Invoke();
            }
            else
            {
                ignoreAction?.Invoke();
            }
            inputAmountsum += InputAmount;
            InputAccount = "";
            InputAmount = 0;
            SetText();
            BankMemo.TextSetting("To Owner", RandomAccount, RandomAmount-inputAmountsum);
            ChangeView(0);
            
        }
      
        public void ChangeMenuToTransTransaction(bool isMenu) //하드 코딩
        {
            if (isMenu) //메뉴로 갈 경우
            {
                bankMenuRectTransform.DOLocalMoveX(0, 1f);
                bankTransactionTransform.DOLocalMoveX(600, 1f);
                CurrentView = 1;
            }
            else
            {
                bankTransactionTransform.anchoredPosition = new Vector2(600,0);
                bankMenuRectTransform.DOLocalMoveX(-600, 1f);
                bankTransactionTransform.DOLocalMoveX(0, 1f);
                CurrentView = 10;
            }

        }

        public void ChangeView(int index) // 임시 화면 전환
        {
            void Move(int startIndex)
            {
                password.transform.DOLocalMoveX(600 * startIndex++, 1f);
                bankMenuRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankAccountRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankAmountRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankFinishRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
                bankResultRectTransform.DOLocalMoveX(600 * startIndex++, 1f);
            }
            bankTransactionTransform.anchoredPosition = new Vector2(600, -960);  // 거래내역 화면 제자리로 보내기 하드코딩...

            Move(-index);
            if (index == 4) 
            {
                SetAccount();
                KeyPadMove(false);
            }
            if(index == 1)
                KeyPadMove(false);
            CurrentView = index;
        }

        public void SetText()
        {
            string pre = InputAccount;
            string pre1 = InputAmount.ToString();
            InputAccountText.text = AddBar(pre);
            InputAmountText.text = AddCommas(pre1) + "원";
        }
        public void InputClickButton(int num)
        {
            if (CurrentView == 2) //계좌 입력
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
            else if (CurrentView == 3) //숫자 입력
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
    public partial class BankApp //새로 추가한 거래내역 확인 
    {
        public TradeHistoryBox[] HistroyBoxs = new TradeHistoryBox[2]; // 입금하기 전 거래 내역 = 최근 거래내역이 없습니다. 
        public TextMeshProUGUI depositText; //입급한 금액
        public TextMeshProUGUI withdrawalText;
       
        public void TradeHistoryInit() //거래 내역 초기화
        {
            HistroyBoxs[0].BoxDataSetting(0, 123); //123 더미데이터
            HistroyBoxs[1].BoxDataSetting(-1, 123); //123 더미데이터
        }

        //HistoryUpload(2 , 100000); 입금 100000표시   
        public void HistoryUpload(int type , int amount)
        {
            HistroyBoxs[1].BoxDataCopy(HistroyBoxs[0]); // 아래로 보내기

            HistroyBoxs[0].BoxDataSetting(type, amount);
            // -1=빈박스/  0= 거래내역이 없습니다. / 1= 출금 / 2= 입금
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
        public AppState AppState { get; set; }

        public void SetActiveBackground(bool value)
        {
            mainCanvas.gameObject.SetActive(value);
            BankMemo.gameObject.SetActive(value);
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;
            mainCanvas.worldCamera = phone.phoneCamera;
            password.phone = phone;

            BankMemo.gameObject.layer = 0;
            BankMemo.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(false);

            var home = _phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            appButton.button.interactable = false;
            
            home.appGridControl.Insert(appButton, new Vector2Int(2,0));
        }

        public void AppPlay(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(true);
            password.phone = phone;

            BankMemo.ShowAnimation();
        }

        public void AppResume(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(true);

            BankMemo.ShowAnimation();
        }

        public void AppPause(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(false);
            BankMemo.HideAnimation();
        }

        public void AppExit(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(false);
            BankMemo.HideAnimation();
        }

        public void AppUnInstall(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(false);
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

            if (!EditorApplication.isPlaying) return;
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
