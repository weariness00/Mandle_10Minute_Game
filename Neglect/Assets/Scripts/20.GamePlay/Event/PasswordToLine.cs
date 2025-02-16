using GamePlay.Phone;
using Manager;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace GamePlay.Event
{
    public class PasswordToLine : MonoBehaviour
    {
        public PhoneControl phone;
        public Action ClearAction;
        public RectTransform canvasRect;
        [Header("정답 패스워드")]
        public List<int> AnswerPassword;
        [Space]
        // Start is called before the first frame update
        [Tooltip("패스워드 점들")]
        public List<GameObject> PasswordPointers = new List<GameObject>(); // 패스워드 점들

        [Tooltip("패스워드 라인")]
        public RectTransform[] PasswordLine = new RectTransform[10];

        public List<int> InputPassword = new List<int>(); //현재 입력받은 패스워드

        [Tooltip("드래그 탐지 범위")]
        public double DetectedRange = 0.3f; // 점과 마우스사이 탐지 범위
        private bool IsCrack = false; // 패스워드 푸는 중인지 
        private int CurrentView = 0; //현재 화면

        public TextMeshProUGUI HintText;


        public void Awake()
        {
            Init();
        }
        //패스워드
        public void Init()
        {
            IsCrack = false;
            InputPassword.Clear();
            LineClear();
        }

        public void SettingEvent(string hint , string Password)
        {
            HintText.text = hint;
            AnswerPassword.Clear();
            string[] strNumbers = Password.Trim(new char[] { '[', ']' }).Split(',');
            int[] numbers = Array.ConvertAll(strNumbers, int.Parse);
           
            for (int i = 0; i < numbers.Length; i++)
            {
                AnswerPassword.Add(numbers[i] - 1); // 인덱스 번호로 제작되었으므로 -1 처리 
            }
        }

        public void LineDraw()
        {
            for (int i = 1; i < InputPassword.Count; i++)
            {
                PasswordLine[i].gameObject.SetActive(true);
                SetLine(PasswordLine[i], PasswordPointers[InputPassword[i - 1]].transform.localPosition, PasswordPointers[InputPassword[i]].transform.localPosition);
                // PasswordLine[i].SetPosition(1, PasswordPointers[InputPassword[i]].transform.position);
            }

            PasswordLine[InputPassword.Count].gameObject.SetActive(true);
   
            SetLine(PasswordLine[InputPassword.Count], PasswordPointers[InputPassword[InputPassword.Count-1]].transform.localPosition, GetMousePositionInCanvas());

        }
        public Vector2 GetMousePositionInCanvas()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue(); // 마우스 위치 (스크린 좌표)

            Vector2 localPoint;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //     canvasRect,  // 캔버스의 RectTransform
            //     mousePos,     // 현재 마우스 스크린 위치
            //     Camera.main,         // 카메라 (World Space Canvas라면 Camera.main)
            //     out localPoint
            // );
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,  // 캔버스의 RectTransform
                phone.phoneMousePosition,     // 현재 마우스 스크린 위치
                phone.phoneCamera,         // 카메라 (World Space Canvas라면 Camera.main)
                out localPoint
            );
            return localPoint; // UI 내에서의 로컬 좌표 반환
        }
        public void SetLine(RectTransform lineImage, Vector3 pos1, Vector3 pos2)
        {
            if (lineImage == null)
            {
                Debug.LogError("Line Image가 설정되지 않았습니다!");
                return;
            }

            lineImage.anchoredPosition = (pos1 + pos2) / 2;
            float distance = Vector3.Distance(pos1, pos2);
            lineImage.sizeDelta = new Vector2(Mathf.Abs(distance), lineImage.rect.size.y); // width=길이, height=두께

            Vector3 direction = (pos2 - pos1).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineImage.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void Update()
        {
            if (Mouse.current.leftButton.isPressed && IsCrack)
            {
                LineDraw();
                
            }
            
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {

                IsCrack = false;
                if (PasswordCheck())
                {
                    ClearAction();  //클리어
                    //Destroy(gameObject);
                }
                InputPassword.Clear();
                LineClear();
            }
        }
        public void CheckSkipNumber() // 비정상적인 패스워드 입력시 수정
        {
            int BackIndex = InputPassword.Count - 1;
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
            int[,] C = new int[,] { { 0, 2 }, { 3, 5 }, { 6, 8 }, { 0, 8 }, { 2, 6 }, { 0, 6 }, { 1, 7 }, { 2, 8 } };
            for (int i = 0; i < 8; i++)
            {
                if (A == C[i, 0] && B == C[i, 1] || B == C[i, 0] && A == C[i, 1])
                    return true;
            }
            return false;

        }
        public void LineClear()
        {
            for (int i = 0; i < 10; i++)
            {
                PasswordLine[i].gameObject.SetActive(false);
            }
        }  //라인 클리어
        public bool PasswordCheck()  //입력 비밀번호랑 정답 비밀번호랑 비교
        {
            bool flag = true;
            if (AnswerPassword.Count != InputPassword.Count)
            {
                return false;
            }

            for (int i = 0; i < InputPassword.Count; i++)
            {
                if (InputPassword[i] != AnswerPassword[i])
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }


        public void DownAddPoint(int num)
        {
            InputPassword.Clear();
            InputPassword.Add(num);
            IsCrack = true;
        }
        public void DragAddPoint(int num)
        {
            if (IsCrack && !InputPassword.Contains(num) && !InputPassword.Contains(num))
            {
                InputPassword.Add(num);

                CheckSkipNumber();
            }
        }


        /// 패스워드


    }
}