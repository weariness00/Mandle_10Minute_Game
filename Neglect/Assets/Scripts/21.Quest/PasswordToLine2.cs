using Manager;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PasswordToLine2 : MonoBehaviour
{


    [Header("은행 화면 정보")]
    public GameObject BankPassword;
    public GameObject BankTransfer;
    public GameObject BankFinish;
    [Space]


    [Header("이체 해야 할 정보")]
    public string AnswerAccount;
    public int AnswerAmount;
    public string PassbookOwner;
    [Space]
    [Header("정답 패스워드")]
    public List<int> AnswerPassword;
    [Space]


    // Start is called before the first frame update
    [Tooltip("패스워드 점들")]
    public List<GameObject> PasswordPointers = new List<GameObject>(); // 패스워드 점들

    [Tooltip("패스워드 라인")]
    public LineRenderer[] PasswordLine = new LineRenderer[10];

    public List<int> InputPassword = new List<int>(); //현재 입력받은 패스워드

    [Tooltip("드래그 탐지 범위")]
    public double DetectedRange = 0.3f; // 점과 마우스사이 탐지 범위
    private bool IsCrack = false; // 패스워드 푸는 중인지 
    private int CurrentView = 0; //현재 화면


    [Header("패스워드 완료 후 계좌 이체 텍스트")]
    public TextMeshProUGUI InputAccount;
    public TextMeshProUGUI InputAmount;

    public MMF_Player pre_sign;

    [Header("마지막 확인 텍스트")]
    public TextMeshProUGUI CheckText; 
    public TextMeshProUGUI CheckAmountText;

    
    //패스워드
    public void Init()
    {
        IsCrack = false;
        InputPassword.Clear();
        LineClear();
    }
    public void RePositionPointers()
    {

    }
    public void LineDraw()
    {
        for (int i = 1; i < InputPassword.Count; i++)
        {
            PasswordLine[i].positionCount = 2;
            PasswordLine[i].SetPosition(0, PasswordPointers[InputPassword[i-1]].transform.position);
            PasswordLine[i].SetPosition(1, PasswordPointers[InputPassword[i]].transform.position);
        }
        Vector3 curmouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        curmouse.z = 0;
        PasswordLine[InputPassword.Count].positionCount = 2;
        PasswordLine[InputPassword.Count].SetPosition(0, PasswordPointers[InputPassword[InputPassword.Count - 1]].transform.position);
        PasswordLine[InputPassword.Count].SetPosition(1, curmouse);
        
    }
    public void Update()
    {
        if (CurrentView != 0)
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            IsCrack = StartCrack();
        }

        if (Mouse.current.leftButton.isPressed && IsCrack)
        {
            LineDraw();
            int AddPointer = DetectedPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            if (AddPointer >= 0)
            {
                InputPassword.Add(AddPointer);
                CheckSkipNumber();
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            IsCrack = false;
            if(PasswordCheck())
                ChangeView(1); // 다음화면
            InputPassword.Clear();
            LineClear();

        }
    }
    public void CheckSkipNumber() // 비정상적인 패스워드 입력시 수정
    {
        
        int BackIndex = InputPassword.Count-1;
       if (IsSkip())
        {
            int storeIndex = InputPassword[BackIndex];
            int MiddleNumber = (InputPassword[BackIndex] + InputPassword[BackIndex - 1]) / 2;
            if (!InputPassword.Contains(MiddleNumber))
            {
                InputPassword[BackIndex] = (InputPassword[BackIndex] + InputPassword[BackIndex - 1]) / 2;
                InputPassword.Add(storeIndex);
            }

        }
    }
    public bool IsSkip() // 비정상적인 패스워드 입력 체크
    {
        int BackIndex = InputPassword.Count - 1;
        int A = InputPassword[BackIndex];
        int B = InputPassword[BackIndex - 1];
        int[,] C = new int[,] { { 0, 2 }, { 3, 5 }, { 6, 8 }, { 0, 8 }, { 2, 6 } , { 0, 6 }, { 1, 7 }, { 2, 8 } };
        for(int i = 0; i < 8; i++)
        {
            if ((A == C[i, 0] && B == C[i, 1]) || (B == C[i, 0] && A == C[i, 1]))
                return true;
        }
        return false;

    }
    public void LineClear()
    {
        for (int i = 0; i < 10; i++)
        {
            PasswordLine[i].positionCount = 0;
        }
    }  //라인 클리어
    public bool PasswordCheck()  //입력 비밀번호랑 정답 비밀번호랑 비교
    {
        bool flag = true;
        if (AnswerPassword.Count != InputPassword.Count)
        {
            return false;
        }

        for(int i = 0;  i < InputPassword.Count; i++)
        {
            if (InputPassword[i] != AnswerPassword[i])
            {
                flag = false;
                break;
            }
        }

        return flag;
    }
    public bool StartCrack()
    {
        int StartNum = DetectedPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        if(StartNum >= 0)
        {
            InputPassword.Add(StartNum);
            return true;
        }
        return false;
    } // 선 잇기 시작
    public int DetectedPoint(Vector3 MousePosition) //가장 가까운 점 인덱스 반환 없으면 -1
    {
        MousePosition.z = 0;        
        float MinDistance = 9999999f;
        float PreDistance = 9999999f;
        int ClosePointerIndex = -1;
        for (int i = 0; i < PasswordPointers.Count; i++)
        {
            if (InputPassword.Contains(i))//이미 입력된 숫자 제외
                continue;

            PreDistance = Vector2.Distance(MousePosition, PasswordPointers[i].transform.position);
            if (MinDistance > PreDistance)
            {
                ClosePointerIndex = i;
                MinDistance = PreDistance;
            }
        }

        if (MinDistance <= DetectedRange)
        {
            return ClosePointerIndex;
        }

        return -1;
    }
    /// 패스워드

   
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
        input= Regex.Replace(input, @"\u200B", "");
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
        int Amountdifference = AnswerAmount - int.Parse(inputText);
        Debug.Log(Amountdifference + "만큼 금액 차이 발생");
    }
    

    public void ChangeView(int index) // 임시 화면 전환
    {
        if(index == 0)
        {
            BankPassword.SetActive(true);
            BankTransfer.SetActive(false);
            BankFinish.SetActive(false);
        }
        if (index == 1)
        {
            BankPassword.SetActive(false);
            BankTransfer.SetActive(true);
            BankFinish.SetActive(false);
        }
        if (index == 2)
        {
            BankPassword.SetActive(false);
            BankTransfer.SetActive(false);
            BankFinish.SetActive(true);
        }
        CurrentView = index;
    }
}
