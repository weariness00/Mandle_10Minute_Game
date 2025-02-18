using DG.Tweening;
using GamePlay.Talk;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Event
{
    public class CallConversation : MonoBehaviour
    {
        // Start is called before the first frame update

        public string CurrentName = "npc";
        public TextMeshProUGUI ChatName;



        [Space]
        public TalkingData talkData;

        [Header("말풍선")]
        [Tooltip("상대방 말풍선")]
        public TextMeshProUGUI OtherText;
        public Image OtherChat;

        [Tooltip("내 말풍선")]
        public TextMeshProUGUI MyText;
        public Image MyChat;

        [Space]
        [Header("선택지 버튼")]
        public List<Button> SelectButtons;
        public List<Image> SelectImages;
        public List<TextMeshProUGUI> SelectTexts;


        public RectTransform OtherTextBoxRect;
        public RectTransform MyTextBoxRect;
        [Space]
        [Header("게이지")]
        public float ChatGage;
        public Image GageBar;

        [Header("데이터베이스에서 가져올 정보")]
        [Space]
        [Tooltip("상대방 질문")]
        public string Question;
        [Tooltip("답변 list")]
        public string[] replyString;
        [Tooltip("답변 게이지list")]
        public int[] replygage;
        [Tooltip("답변 후속 질문 이벤트")]
        public string[] replyEvent;

        public bool isClickButton;

        public Action ClearAction;

        public TextMeshProUGUI TimeText;

        public void Awake()
        {

            ChatName.text = CurrentName;
           
        }
        public void Start()
        {
            ChatStart();
        }
        public void SettingReply()
        {
            if (talkData == null)
            {
                Debug.LogError("talkData is null");
                return; // talkData가 null이면 이 함수의 실행을 중단
            }
            if (talkData.negativeTextArray.Length + talkData.positiveTextArray.Length == 0)
            {
                return; // talkData가 null이면 이 함수의 실행을 중단
            }
            List<string> combinedList = new List<string>(talkData.positiveTextArray);
            combinedList.AddRange(talkData.negativeTextArray);
            replyString = combinedList.ToArray();
            int count = 0;
            
            for (int i = 0; i < talkData.positiveTextArray.Length; i++)
            {
                replygage[count] = 20;
                count++;
            }
            for (int i = 0; i < talkData.negativeTextArray.Length; i++)
            {
                replygage[count] = 0;
                count++;
            }
            for (int i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, 3);
                (replyString[i], replyString[index]) = (replyString[index], replyString[i]);
                (replygage[i], replygage[index]) = (replygage[index], replygage[i]);
            }
        }

        private float timer = 0f;
        void Update()
        {
            timer += Time.deltaTime; // 초 단위 증가
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            TimeText.text = $"{minutes:00}:{seconds:00}"; // 00:00 형식
        }
        public void ResetObject()
        {
            OtherChat.gameObject.SetActive(false);
            OtherText.gameObject.SetActive(false);
            MyChat.gameObject.SetActive(false);
            MyText.gameObject.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                SelectTexts[i].gameObject.SetActive(false);
                SelectButtons[i].gameObject.SetActive(false);
            }
        }

        public void ChatStart()
        {
            isClickButton = false;
            ResetObject();
            SettingReply();
            // 데이터베이스 재설정 코드 넣을 것.

            Sequence UiSeq = DOTween.Sequence();
            UiSeq.AppendCallback(() =>
            {
                OtherChat.gameObject.SetActive(true);
            });
            UiSeq.Append(OtherChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(OtherChat.DOFade(0f, 0f)).Join(OtherChat.DOFade(1f, 0.5f));
            UiSeq.AppendCallback(() =>
            {
                OtherText.gameObject.SetActive(true);
                OtherText.text = talkData != null ? talkData.mainText : "Test";
            });
            // ~ 상대 말풍선 애니메이션


            UiSeq.AppendCallback(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    SelectButtons[i].interactable = true;
                    SelectButtons[i].gameObject.SetActive(true);
                }
            });
            for (int i = 0; i < 3; i++)
            {
                SelectImages[i].DOFade(0f, 0f);
                UiSeq.Append(SelectButtons[i].gameObject.transform.DOLocalMoveY(SelectButtons[i].gameObject.transform.localPosition.y - 10f, 0.5f)
                .From()).Join(SelectImages[i].DOFade(1f, 0.5f));
            }
            UiSeq.AppendCallback(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    SelectTexts[i].gameObject.SetActive(true);
                    Color reset = SelectTexts[i].color;
                    reset.a = 1f;
                    SelectTexts[i].color = reset;
                    SelectTexts[i].text = replyString[i];
                }
            });
            UiSeq.AppendCallback(() =>
            {
                isClickButton = true;
            });
            //~ 버튼 나오는 애니메이션
        }

        public void ChoiceBttons(int index) //버튼 클릭시
        {
            if (!isClickButton)
                return;
            Sequence UiSeq = DOTween.Sequence();

            for (int i = 0; i < 3; i++)
            {
                SelectButtons[i].interactable = false;
                if (i != index)
                    UiSeq.Join(SelectImages[i].DOFade(0f, 0.5f)).Join(SelectTexts[i].DOFade(0f, 0.5f));
            }
            UiSeq.Append(SelectImages[index].DOFade(0f, 0.5f)).Join(SelectTexts[index].DOFade(0f, 0.5f));
            // ~버튼 종료 애니메이션



            UiSeq.AppendCallback(() =>
            {
                MyChat.gameObject.SetActive(true);
            });
            UiSeq.Append(MyChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(MyChat.DOFade(0f, 0f)).Join(MyChat.DOFade(1f, 0.5f));
            UiSeq.AppendCallback(() =>
            {
                MyText.gameObject.SetActive(true);
                MyText.text = replyString[index];
            });
            // ~ 내 채팅 나오는 애니메이션


            ChatGage = ChatGage + replygage[index] > 100 ? 100 : ChatGage + replygage[index];
            ChatGage = ChatGage < 0 ? 0 : ChatGage;

            UiSeq.Append(GageBar.DOFillAmount(ChatGage / 100f, 1f)).OnComplete(()=> { 
            
            
            
            });
            // 게이지 차는 애니메이션


            if (ChatGage < 100)
                UiSeq.AppendCallback(() => ChatStart()); // 반복
            else
            {
                UiSeq.AppendCallback(() =>
                {
                    ClearAction();
                    Destroy(gameObject);
                    Debug.Log("이벤트 종료");
                });
            }
        }
    }
}