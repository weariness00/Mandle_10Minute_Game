using DG.Tweening;
using GamePlay.Talk;
using Manager;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

namespace GamePlay.Event
{
    public class CallConversation : MonoBehaviour
    {
        // Start is called before the first frame update

        public string CurrentName = "npc";
        public TextMeshProUGUI ChatName;
        public bool isComplete;
        public int choiceIndex;

        public AudioSource EndSound;

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

        public CallconversationTextBox OtherTextBoxScript;
        public CallconversationTextBox MyTextBoxScript;
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
        public int[] replyEvent;


        private int ReplyCount;
        public bool isClickButton;
        public Action ClearAction; // 클리어했을 때 호출
        public Action onIgnoreEvent;
        public TextMeshProUGUI TimeText; //타이머 텍스트

        public MMF_Player CloseCall;
        public MMF_Player CloseMyText;
        public Button CallEndButton;

        private TalkingData checkTalkdata;
        public void Awake()
        {
            ChatName.text = CurrentName;
            CallEndButton.gameObject.SetActive(false);
            //OtherTextBoxScript.isEndAnimation += ShowSelectButton; //상대방 텍스트 나레이션 끝나면 버튼이 나오도록
            //MyTextBoxScript.isEndAnimation += FillGage; //자신의 텍스트 나레이션 끝나면 게이지가 채워지도록

        }

        public void Start()
        {
            ChatStart();
        }

        public void SetTalkData(TalkingData data)
        {
            if (data == null || talkData == data) return;
            talkData = data;
        }
        public void SettingReply()
        {
            if (talkData == null)
            {
                return; // talkData가 null이면 이 함수의 실행을 중단
            }
            if (talkData.negativeTextArray.Length + talkData.positiveTextArray.Length == 0)
            {
                return; // talkData가 null이면 이 함수의 실행을 중단
            }

            ReplyCount = 0;
            List<string> combinedList = new List<string>(talkData.positiveTextArray);
            combinedList.AddRange(talkData.negativeTextArray);
            replyString = combinedList.ToArray();


            for (int i = 0; i < talkData.positiveTextArray.Length; i++)
            {
                replygage[ReplyCount] = talkData.positiveScore;
                replyEvent[ReplyCount] = talkData.positiveResultTalkID;
                ReplyCount++;
            }
            for (int i = 0; i < talkData.negativeTextArray.Length; i++)
            {
                replygage[ReplyCount] = talkData.negativeScore;
                replyEvent[ReplyCount] = talkData.negativeResultTalkID;
                ReplyCount++;
            }
            for (int i = 0; i < ReplyCount; i++)
            {
                int index = UnityEngine.Random.Range(0, ReplyCount);
                (replyString[i], replyString[index]) = (replyString[index], replyString[i]);
                (replygage[i], replygage[index]) = (replygage[index], replygage[i]);
                (replyEvent[i], replyEvent[index]) = (replyEvent[index], replyEvent[i]);

            }
        }

        private float timer = 0f;
        void Update()
        {
            if ((isComplete))
                return;
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
        public void TalkTextSetting(TextMeshProUGUI Boxs, string text)
        {
            string Result = "";
            for (int i = 0; i < text.Length; i++)
            {
                Result += text[i];
                if (i % 10 == 0 && i != 0)
                {
                    Result += "\n";
                }
            }
            Boxs.text = Result;
        }
        public void ChatStart()
        {
            if (MyTextBoxRect == null)
                MyTextBoxRect = MyChat.GetComponent<RectTransform>();
            if (OtherTextBoxRect == null)
                OtherTextBoxRect = OtherChat.GetComponent<RectTransform>();

            isClickButton = false;
            ResetObject();
            SettingReply();
            // 데이터베이스 재설정 코드 넣을 것.

            Sequence UiSeq = DOTween.Sequence();
            UiSeq.AppendCallback(() =>
            {
                OtherChat.gameObject.SetActive(true);
                OtherText.gameObject.SetActive(true);
                OtherText.text = "";
            });
            UiSeq.Append(OtherChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(OtherChat.DOFade(0f, 0f)).Join(OtherChat.DOFade(1f, 0.5f));
            // ~ 상대 말풍선 애니메이션
            UiSeq.AppendCallback(() =>
            {
                OtherTextBoxScript.SetNarration(talkData != null ? talkData.mainText : "Test", 17, ShowSelectButton); //가 끝나면 showSelectButton 실행
            });
        }
        public void ShowSelectButton()
        {
            Sequence UiSeq = DOTween.Sequence();
            UiSeq.AppendCallback(() =>
            {
                for (int i = 0; i < ReplyCount; i++)
                {
                    SelectButtons[i].interactable = true;
                    SelectButtons[i].gameObject.SetActive(true);
                }
            });
            for (int i = 0; i < ReplyCount; i++)
            {
                SelectImages[i].DOFade(0f, 0f);
                UiSeq.Append(SelectButtons[i].gameObject.transform.DOLocalMoveY(SelectButtons[i].gameObject.transform.localPosition.y - 10f, 0.3f)
                .From()).Join(SelectImages[i].DOFade(1f, 0.3f));
            }
            UiSeq.AppendCallback(() =>
            {
                for (int i = 0; i < ReplyCount; i++)
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


            choiceIndex = index;
            Sequence UiSeq = DOTween.Sequence();

            for (int i = 0; i < 3; i++)
            {
                SelectButtons[i].interactable = false;
                if (i != index)
                    UiSeq.Join(SelectImages[i].DOFade(0f, 0.3f)).Join(SelectTexts[i].DOFade(0f, 0.3f));
            }
            UiSeq.Append(SelectImages[index].DOFade(0f, 0.3f)).Join(SelectTexts[index].DOFade(0f, 0.3f));
            // ~버튼 종료 애니메이션

            UiSeq.AppendCallback(() =>
            {
                MyChat.gameObject.SetActive(true);
                MyText.gameObject.SetActive(true);
                MyText.text = "";
            });
            UiSeq.Append(MyChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(MyChat.DOFade(0f, 0f)).Join(MyChat.DOFade(1f, 0.5f));
            UiSeq.AppendCallback(() =>
            {
                MyTextBoxScript.SetNarration(replyString[index], 17, FillGage);
            });


            // ~ 내 채팅 나오는 애니메이션
        }
        public void FillGage()
        {
            Sequence UiSeq = DOTween.Sequence();

            ChatGage = ChatGage + replygage[choiceIndex] > 100 ? 100 : ChatGage + replygage[choiceIndex];
            ChatGage = ChatGage < 0 ? 0 : ChatGage;

            UiSeq.Append(GageBar.DOFillAmount(ChatGage / 100f, 1f));
            // 게이지 차는 애니메이션
            if (ChatGage < 100)
                UiSeq.AppendCallback(() => {
                    SetTalkData(TalkingScriptableObject.Instance.GetTalkData(replyEvent[choiceIndex]));
                    ChatStart();
                }); // 반복
            else
            {
                UiSeq.AppendCallback(() =>
                {
                    mainTextexist();
                });
            }
        }


        public void mainTextexist()
        {
            Sequence UiSeq = DOTween.Sequence();
            checkTalkdata = talkData;
            SetTalkData(TalkingScriptableObject.Instance.GetTalkData(replyEvent[choiceIndex]));
            if(talkData == checkTalkdata)
            {
                CallEndButton.gameObject.SetActive(true);
                return;
            }


            UiSeq.AppendCallback(() =>
            {

                MyChat.gameObject.SetActive(false);
                MyText.gameObject.SetActive(false);
                OtherText.text = "";
            });
            UiSeq.Append(OtherChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(OtherChat.DOFade(0f, 0f)).Join(OtherChat.DOFade(1f, 0.5f));
            // ~ 상대 말풍선 애니메이션
            UiSeq.AppendCallback(() =>
            {
                OtherTextBoxScript.SetNarration(talkData != null ? talkData.mainText : "Test", 17, null); //가 끝나면 showSelectButton 실행
            });
            UiSeq.AppendCallback(() =>
            {
                CallEndButton.gameObject.SetActive(true);
            });
        }


        public void CallEndAnimation()
        {
            isComplete = true;
            CallEndButton.interactable = false;
            CloseCall.Events.OnComplete.AddListener(() =>
            {
                ClearAction?.Invoke();
                Destroy(gameObject);
            });
            if (checkTalkdata == talkData)
                CloseMyText.PlayFeedbacks();
            CloseCall.PlayFeedbacks();
            EndSound.Play();
            if(!TimeText.text.Contains("\n통화종료"))
                TimeText.text += "\n통화종료";
        }
    }
}