using GamePlay.Phone;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GamePlay.Event
{
    public class PasswordToLine : MonoBehaviour
    {
        public PhoneControl phone;
        public Action ClearAction;
        public RectTransform canvasRect;
        [Header("정답 패스워드")]
        public List<int> answerPassword;
        [Space]
        // Start is called before the first frame update
        [Tooltip("패스워드 점들")]
        public List<GameObject> PasswordPointers = new List<GameObject>(); // 패스워드 점들

        [Tooltip("패스워드 라인")]
        public RectTransform[] PasswordLine = new RectTransform[10];

        public List<int> inputPassword = new List<int>(); //현재 입력받은 패스워드

        [Tooltip("드래그 탐지 범위")]
        public double DetectedRange = 0.3f; // 점과 마우스사이 탐지 범위
        private bool IsCrack = false; // 패스워드 푸는 중인지 
        private int CurrentView = 0; //현재 화면

        public TextMeshProUGUI HintText;

        private bool isInit = false;

        public void Awake()
        {
            inputPassword.Clear();
            LineClear();
        }
        //패스워드
        public void Init()
        {
            isInit = true;
            IsCrack = false;
            inputPassword.Clear();
            LineClear();
        }

        public void SettingEvent(List<int> password)
        {
            var realPassword = new List<int>(password);

            // 패스워드의 중간값을 찾아 넣어준다.
            void InsertPasswordBetween(int lastIndex, int value)
            {
                // 1 -> 3 이면 1 -> 2 -> 3 이렇게 되게 해준다.
                bool isHas = false;
                for (int i = 0; i < lastIndex - 1; i++)
                {
                    if (password[i] == value)
                    {
                        isHas = true;
                        break;
                    }
                }

                // 1 -> 2 -> 3 -> 2 -> 4 라고 되어 있으면 1 -> 2 -> 3 -> 4 으로 바꾸어준다.
                for (int i = lastIndex + 1; i < password.Count; i++)
                {
                    if (password[i] == value)
                    {
                        password.RemoveAt(i);
                        realPassword.Remove(value); // 본래 패스워드에서 지워주기
                        break;
                    }
                }
                if(!isHas)
                    password.Insert(lastIndex + 1, value);
            }
            
            // 모든 중간값 경우의 수
            for (var i = 0; i < password.Count - 1; i++)
            {
                var first = password[i];
                var second = password[i + 1];

                if (first == 1)
                {
                    if (second == 3) InsertPasswordBetween(i, 2);
                    if (second == 7) InsertPasswordBetween(i, 4);
                    if (second == 9) InsertPasswordBetween(i, 5);
                }
                else if (first == 3)
                {
                    if (second == 1) InsertPasswordBetween(i, 2);
                    if (second == 7) InsertPasswordBetween(i, 6);
                    if (second == 9) InsertPasswordBetween(i, 5);
                }
                else if (first == 7)
                {
                    if (second == 1) InsertPasswordBetween(i, 4);
                    if (second == 3) InsertPasswordBetween(i, 5);
                    if (second == 9) InsertPasswordBetween(i, 8);
                }
                else if (first == 9)
                {
                    if (second == 1) InsertPasswordBetween(i, 5);
                    if (second == 3) InsertPasswordBetween(i, 6);
                    if (second == 7) InsertPasswordBetween(i, 8);
                }
                else if (first == 2)
                {
                    if (second == 8) InsertPasswordBetween(i, 5);
                }
                else if (first == 4)
                {
                    if (second == 6) InsertPasswordBetween(i, 5);
                }
                else if (first == 6)
                {
                    if (second == 4) InsertPasswordBetween(i, 5);
                }
                else if (first == 8)
                {
                    if (second == 2) InsertPasswordBetween(i, 5);
                }
            }
            
            answerPassword.Clear();
            foreach (var index in password)
                answerPassword.Add(index - 1); // 인덱스 번호로 제작되었으므로 -1 처리 

            HintText.text = "힌트 : ";
            HintText.text += string.Join(" ", realPassword);
        }

        public void LineDraw()
        {
            for (int i = 1; i < inputPassword.Count; i++)
            {
                PasswordLine[i].gameObject.SetActive(true);
                SetLine(PasswordLine[i], PasswordPointers[inputPassword[i - 1]].transform.localPosition, PasswordPointers[inputPassword[i]].transform.localPosition);
                // PasswordLine[i].SetPosition(1, PasswordPointers[InputPassword[i]].transform.position);
            }

            PasswordLine[inputPassword.Count].gameObject.SetActive(true);
   
            SetLine(PasswordLine[inputPassword.Count], PasswordPointers[inputPassword[inputPassword.Count-1]].transform.localPosition, GetMousePositionInCanvas());

        }
        public Vector2 GetMousePositionInCanvas()
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,  // 캔버스의 RectTransform
                phone ? phone.phoneMousePosition : Mouse.current.position.ReadValue(),     // 현재 마우스 스크린 위치
                phone ? phone.phoneCamera : Camera.main,         // 카메라 (World Space Canvas라면 Camera.main)
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
            if(!isInit) return;
            
            if (Mouse.current.leftButton.isPressed && IsCrack)
            {
                LineDraw();
                
            }
            
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                IsCrack = false;
                if (PasswordCheck())
                {
                    ClearAction?.Invoke();  //클리어
                    //Destroy(gameObject);
                }
                inputPassword.Clear();
                LineClear();
            }
        }
        public void CheckSkipNumber() // 비정상적인 패스워드 입력시 수정
        {
            int BackIndex = inputPassword.Count - 1;
            if (IsSkip())
            {
                int storeIndex = inputPassword[BackIndex];
                int MiddleNumber = (inputPassword[BackIndex] + inputPassword[BackIndex - 1]) / 2;
                if (!inputPassword.Contains(MiddleNumber))
                {
                    inputPassword[BackIndex] = (inputPassword[BackIndex] + inputPassword[BackIndex - 1]) / 2;
                    inputPassword.Add(storeIndex);
                }

            }
        }
        public bool IsSkip() // 비정상적인 패스워드 입력 체크
        {
            int BackIndex = inputPassword.Count - 1;
            int A = inputPassword[BackIndex];
            int B = inputPassword[BackIndex - 1];
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
            if (answerPassword.Count != inputPassword.Count)
            {
                return false;
            }

            for (int i = 0; i < inputPassword.Count; i++)
            {
                if (inputPassword[i] != answerPassword[i])
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }


        public void DownAddPoint(int num)
        {
            inputPassword.Clear();
            inputPassword.Add(num);
            IsCrack = true;
        }
        public void DragAddPoint(int num)
        {
            if (IsCrack && !inputPassword.Contains(num) && !inputPassword.Contains(num))
            {
                inputPassword.Add(num);

                CheckSkipNumber();
            }
        }


        /// 패스워드


    }
}