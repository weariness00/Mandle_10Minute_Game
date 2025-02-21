using DG.Tweening;
using GamePlay.Talk;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.UI;
using Util;


namespace GamePlay.Chatting
{
    public partial class Conversation
    {
        private ChatTextBox otherChat;
        public void Awake()
        {
            talkData = null;
            ChatName.text = CurrentName;

            answerBlockPool = new(
                () =>
                {
                    var block = Instantiate(answerBlockPrefab, answerGroupTransform);
                    block.canvasGroup.alpha = 0;
                    block.button.onClick.AddListener(()=>ChoiceButton(block));
                    PhoneUtil.SetLayer(block);
                    return block;
                },
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                obj => Destroy(obj.gameObject),
                true,
                3,
                5
            );
            
        }

        public void Init()
        {
            if (isInit) return;
            backButton.gameObject.SetActive(false);
            isInit = true;
            ignoreTimer.SetMax();
            ignoreEvent = new();
            completeEvent = new();
        }
        
        public void SettingAnswer()
        {
            // pool 비우기
            foreach (AnswerBlock answerBlock in answerList)
                answerBlockPool.Release(answerBlock);
            answerList.Clear();
            
            // 답변 셋팅
            foreach (string text in talkData.positiveTextArray)
            {
                var answer = answerBlockPool.Get();
                answer.answerText.text = text;
                answer.gage = talkData.positiveScore;
                answerList.Add(answer);
            }
            foreach (string text in talkData.negativeTextArray)
            {
                var answer = answerBlockPool.Get();
                answer.answerText.text = text;
                answer.gage = talkData.negativeScore;
                answerList.Add(answer);
            }
            
            // 위치 셔플
            answerList.Shuffle();
            for (var i = 0; i < answerList.Count; i++)
                answerList[i].transform.SetSiblingIndex(i);
        }

        public void SetTalkData(TalkingData data)
        {
            if (data == null || prevTalkData == data) return;
            talkData = data;
            SettingAnswer();
            otherChat = SpawnOtherChat(talkData.mainText);
        }

        public void StartConversation()
        {
            if (!isInit || !gameObject.activeSelf) return;
            if (talkData == null || prevTalkData == talkData) return;
            
            ShowOtherChat();
            ShowMyAnswerList();
            ConversationClear();
        }

        
        public ChatTextBox SpawnOtherChat(string text)
        {
            var otherChat = Instantiate(messageBoxPrefab,Vector3.zero, messageBoxPrefab.transform.rotation, ChatScrollBox.gameObject.transform);
            otherChat.image.sprite = otherMessageSprite;
            otherChat.SetText(text);

            return otherChat;
        }

        public ChatTextBox SpawnAnswerChat(string text)
        {
            var answerChat = Instantiate(messageBoxPrefab,Vector3.zero, messageBoxPrefab.transform.rotation, ChatScrollBox.gameObject.transform);
            answerChat.image.sprite = myMessageSprite;
            answerChat.SetText(text);
            
            return answerChat;
        }
        
        public void ShowMyAnswerList()
        {
            Sequence UiSeq = DOTween.Sequence();
            
            foreach (AnswerBlock answerBlock in answerList)
            {
                answerBlock.button.interactable = true;
                UiSeq.Append(answerBlock.rectTransform.DOLocalMoveY(answerBlock.rectTransform.localPosition.y - 30f, 0.5f).From()).Join(answerBlock.canvasGroup.DOFade(1f, 0.5f));
            }
        }

        public void ShowOtherChat()
        {
            if(otherChat == null) return;
            
            Sequence UiSeq = DOTween.Sequence();
            RectTransform preRect = otherChat.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(preRect);
            
            Vector3 vect = new Vector3(50+preRect.rect.size.x / 2 * preRect.localScale.x, -NextTextPosY - preRect.rect.size.y * preRect.localScale.y, 0);
            otherChat.transform.localPosition = vect;
            NextTextPosY += preRect.rect.size.y * preRect.localScale.y + 20;
            
            ChatScrollBox.sizeDelta = new Vector2(0, NextTextPosY + preRect.rect.size.y / 2);
            StartCoroutine(ScrollToBottom());

            UiSeq.Append(otherChat.gameObject.transform.DOLocalMoveY(otherChat.transform.localPosition.y - 50f, 0.5f).From());
        }

    }
    
    public partial class Conversation : MonoBehaviour
    {
        // Start is called before the first frame update

        public Canvas canvas;

        public string CurrentName = "npc";
        public TextMeshProUGUI ChatName;

        [Header("Interface 버튼")] 
        public Button backButton;

        [Header("복사할 메시지")]
        public ChatTextBox messageBoxPrefab;
        public Sprite otherMessageSprite;
        public Sprite myMessageSprite;

        [Space] [Header("선택지 버튼")] 
        public Transform answerGroupTransform;
        public AnswerBlock answerBlockPrefab;
        private ObjectPool<AnswerBlock> answerBlockPool;   
        private List<AnswerBlock> answerList = new();

        [Space]
        [Header("게이지")]
        public int ChatGage;
        public Image GageBar;

        [Header("데이터베이스에서 가져올 정보")] 
        [Space]
        private TalkingData talkData = null;
        private TalkingData prevTalkData;
        public MinMaxValue<float> ignoreTimer = new(15, 0, 15);
        public UnityEvent completeEvent;
        public UnityEvent ignoreEvent;

        public float NextTextPosY;
        public Scrollbar Scrollbar;
        public RectTransform ChatScrollBox;
        private bool isInit = false;
        public void ConversationClear()
        {
            if (GageBar.fillAmount >= 1 || talkData == null || (talkData.negativeResultTalkID == -1 && talkData.positiveResultTalkID == -1))
            {
                //클리어
                completeEvent?.Invoke();
                isInit = false;
                backButton.gameObject.SetActive(true);
                
                ignoreEvent = new();
                completeEvent = new();
            }
        }
        
        public void MyChatSpawn(string t)
        {
            Sequence UiSeq = DOTween.Sequence();
            var pre = Instantiate(messageBoxPrefab, Vector3.zero, messageBoxPrefab.transform.rotation, ChatScrollBox.gameObject.transform);
            pre.image.sprite = myMessageSprite;
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

        public void ChoiceButton(AnswerBlock block) //버튼 클릭시
        {
            Sequence UiSeq = DOTween.Sequence();
            
            foreach (AnswerBlock answerBlock in answerList)
            {
                answerBlock.button.interactable = false;
                if (answerBlock == block)
                {
                    UiSeq.Append(answerBlock.canvasGroup.DOFade(0f, 0.5f));
                    MyChatSpawn(block.answerText.text);  // ~버튼 종료 애니메이션
                }
                else
                {
                    UiSeq.Append(answerBlock.canvasGroup.DOFade(0f, 0.5f));
                }
            }

            ignoreTimer.Current += 3;
            ChatGage += block.gage;
            UiSeq.Append(GageBar.DOFillAmount(ChatGage / 100f, 1f));
            UiSeq.AppendCallback(() =>
            {
                prevTalkData = talkData;
                talkData = TalkingScriptableObject.Instance.GetTalkData(block.isPositive ? talkData.positiveResultTalkID : talkData.negativeResultTalkID);

                if (talkData != null)
                {
                    SettingAnswer(); 
                    otherChat = SpawnOtherChat(talkData.mainText);
                    ShowMyAnswerList();
                }
                ConversationClear();
            });

        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(Conversation))]
    public class ChatConversationEditor : Editor
    {
        private int talkID;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as Conversation;

            if (EditorApplication.isPlaying)
            {
                talkID = EditorGUILayout.IntField("TalkID", talkID);
                if (GUILayout.Button("Talk 셋팅후 대화 생성"))
                {
                    script.Init();
                    script.SetTalkData(TalkingScriptableObject.Instance.GetTalkData(talkID));
                }
            }
        }
    }

#endif
    
}