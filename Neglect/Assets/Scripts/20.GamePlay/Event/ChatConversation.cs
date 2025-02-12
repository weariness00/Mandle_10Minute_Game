using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Event
{
    public class ChatConversation : MonoBehaviour
    {
        // Start is called before the first frame update

        public string CurrentName = "npc";
        public TextMeshProUGUI ChatName;

        [Header("복사할 메시지")]
        public ChatTextBox OtherMessages;
        public ChatTextBox MyMessages;

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
        
        
        public Action ClearAction;


        public RectTransform ChatLogBox;


        public void Awake()
        {
            ChatName.text = CurrentName;
            ChatStart();
        }
        public void Start()
        {

            for (int i = 0; i < 10; i++)
            {
                if( i%2 == 0)
                    MyChatSpawn("123", i);
                else
                   OtherChatSpawn("2133", i);
            }



        }

        public void SettingDate()
        {
            //대화 내용 최신화
        }

        
        public void OtherChatSpawn(string t , int s)
        {
            var pre = Instantiate(OtherMessages,Vector3.zero, OtherMessages.transform.rotation, ChatLogBox.gameObject.transform);
            pre.SetText(t);
            RectTransform preRect = pre.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(preRect);
            Debug.Log(ChatLogBox.rect.y + preRect.rect.size.y / 2);
            Vector3 vect = new Vector3(50+preRect.rect.size.x / 2, -s* preRect.rect.size.y, 0);
            pre.transform.localPosition = vect;
            ChatLogBox.sizeDelta += new Vector2(0, preRect.rect.size.y);
        }
        public void MyChatSpawn(string t, int s)
        {
            var pre = Instantiate(MyMessages, Vector3.zero, OtherMessages.transform.rotation, ChatLogBox.gameObject.transform);
            pre.SetText(t);
            RectTransform preRect = pre.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(preRect);
            Debug.Log(ChatLogBox.rect.y + preRect.rect.size.y / 2);
            Vector3 vect = new Vector3(550-preRect.rect.size.x / 2, -s * preRect.rect.size.y, 0);
            pre.transform.localPosition = vect;
            
            ChatLogBox.sizeDelta += new Vector2(0, preRect.rect.size.y);



        }




        public void ChatStart()
        {

            // 데이터베이스 재설정 코드 넣을 것.
            /*
            Sequence UiSeq = DOTween.Sequence();
            UiSeq.AppendCallback(() =>
            {
                OtherChat.gameObject.SetActive(true);
            });
            UiSeq.Append(OtherChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(OtherChat.DOFade(0f, 0f)).Join(OtherChat.DOFade(1f, 0.5f));
            UiSeq.AppendCallback(() =>
            {
                OtherText.gameObject.SetActive(true);
                OtherText.text = Question;
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
            //~ 버튼 나오는 애니메이션
            */
        }

        public void ChoiceBttons(int index) //버튼 클릭시
        {
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

            UiSeq.Append(
            GageBar.DOFillAmount(ChatGage / 100f, 1f));
            // 게이지 차는 애니메이션


            if (ChatGage < 100)
                UiSeq.AppendCallback(() => ChatStart()); // 반복
            else
            {
                // 이벤트 종료
                Debug.Log("이벤트 종료");
            }
        }
    }
}