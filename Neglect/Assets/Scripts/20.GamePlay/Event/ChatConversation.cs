using DG.Tweening;
using GamePlay.Talk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public TalkingData talkData;
        [Tooltip("상대방 질문")]
        public string Question;
        [Tooltip("답변 list")]
        public string[] replyString;
        [Tooltip("답변 게이지list")]
        public int[] replygage;
        [Tooltip("답변 후속 질문 이벤트")]
        public string[] replyEvent;
        
        
        public Action ClearAction;


        public float NextTextPosY;
        public Scrollbar Scrollbar;
        public RectTransform ChatScrollBox;

        public void Awake()
        {
            ChatName.text = CurrentName;
        }
        public void Start()
        {

            CallStart();

        }

        
        public void OtherChatSpawn(string t)
        {

            Sequence UiSeq = DOTween.Sequence();
            var pre = Instantiate(OtherMessages,Vector3.zero, OtherMessages.transform.rotation, ChatScrollBox.gameObject.transform);
            pre.SetText(t);
            RectTransform preRect = pre.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(preRect);
            
            Vector3 vect = new Vector3(50+preRect.rect.size.x / 2 * preRect.localScale.x, -NextTextPosY - preRect.rect.size.y * preRect.localScale.y, 0);
            pre.transform.localPosition = vect;
            NextTextPosY += preRect.rect.size.y * preRect.localScale.y + 20;
            
            ChatScrollBox.sizeDelta = new Vector2(0, NextTextPosY + preRect.rect.size.y / 2);
            StartCoroutine(ScrollToBottom());

            UiSeq.Append(pre.gameObject.transform.DOLocalMoveY(pre.transform.localPosition.y - 50f, 0.5f).From());
            
        }
        public void MyChatSpawn(string t)
        {

            Sequence UiSeq = DOTween.Sequence();
            var pre = Instantiate(MyMessages, Vector3.zero, OtherMessages.transform.rotation, ChatScrollBox.gameObject.transform);
            pre.SetText(t);
            RectTransform preRect = pre.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(preRect);
            Vector3 vect = new Vector3(550-preRect.rect.size.x / 2 * preRect.localScale.x, -NextTextPosY - preRect.rect.size.y * preRect.localScale.y, 0);
            pre.transform.localPosition = vect;
            
            NextTextPosY += preRect.rect.size.y * preRect.localScale.y + 20;
            ChatScrollBox.sizeDelta = new Vector2(0, NextTextPosY + preRect.rect.size.y/2);
            StartCoroutine(ScrollToBottom());
            UiSeq.Append(pre.gameObject.transform.DOLocalMoveY(pre.transform.localPosition.y - 50f, 0.5f).From());
        }
        IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame(); // UI 업데이트가 완료된 후 실행
            Scrollbar.value = 0;
        }

        public void CallStart()
        {
            SettingReply();
            OtherChatSpawn(talkData != null ? talkData.mainText : "Test");
            ChatBox();
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

        public void ChatBox()
        {

            Sequence UiSeq = DOTween.Sequence();
            for (int i = 0; i < 3; i++)
            {
                SelectImages[i].DOFade(0f, 0f);

                SelectButtons[i].interactable = true;
                Color reset = SelectTexts[i].color;
                reset.a = 0f;
                SelectTexts[i].color = reset;
                UiSeq.Append(SelectButtons[i].gameObject.transform.DOLocalMoveY(SelectButtons[i].gameObject.transform.localPosition.y - 10f, 0.5f)
                .From()).Join(SelectImages[i].DOFade(1f, 0.5f));
            }
            UiSeq.AppendCallback(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    int index = i;
                    SelectTexts[index].gameObject.SetActive(true);
                    Color reset = SelectTexts[i].color;
                    reset.a = 1f;
                    SelectTexts[index].color = reset;
                    SelectTexts[index].text = replyString[i];
                }
            });
            //~ 버튼 나오는 애니메이션    
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
            MyChatSpawn(replyString[index]);

            ChatGage = ChatGage + replygage[index] > 100 ? 100 : ChatGage + replygage[index];
            ChatGage = ChatGage < 0 ? 0 : ChatGage;
            UiSeq.Append(GageBar.DOFillAmount(ChatGage / 100f, 1f));




            UiSeq.AppendCallback(() =>
            {
                if (ChatGage == 100)
                {
                    //클리어
                    ClearAction();
                    Destroy(gameObject);
                }
                else
                {
                    CallStart();
                }
            });

        }


    }
}