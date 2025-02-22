using DG.Tweening;
using GamePlay.Talk;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Util;


namespace GamePlay.Chatting
{
    public partial class Conversation
    {
        public ScrollRect chatLogScrollRect;
        
        private ChatTextBox otherChatBox;
        private ChatTextBox answerChatBox;

        private bool isChatLogContentSizeUpdate = false;
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
        
        void Update()
        {
            if (isChatLogContentSizeUpdate)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(chatLogScrollRect.content);
                chatLogScrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void Init()
        {
            if (isInit) return;
            backButton.gameObject.SetActive(false);
            isInit = true;
            ignoreTimer.SetMax();
            ignoreEvent = new();
            completeEvent = new();
            chatGage = 0;
            gageBarImage.fillAmount = 0;
            
            // 이전 로그 지워주기
            for (int i = 0; i < chatLogScrollRect.content.childCount; i++)
                Destroy(chatLogScrollRect.content.GetChild(i).gameObject);
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
                answer.isPositive = true;
                answerList.Add(answer);
            }
            foreach (string text in talkData.negativeTextArray)
            {
                var answer = answerBlockPool.Get();
                answer.answerText.text = text;
                answer.gage = talkData.negativeScore;
                answer.isPositive = false;
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
            otherChatBox = SpawnOtherChat(talkData.mainText);
        }

        public void StartConversation()
        {
            if (!isInit || !gameObject.activeSelf) return;
            if (talkData == null || prevTalkData == talkData) return;
            
            // 새로운 대화 시작
            ShowOtherChatBox();
            ShowMyAnswerList();
            ConversationClear();
        }
        
        public ChatTextBox SpawnOtherChat(string text)
        {
            var otherChat = Instantiate(messageBoxPrefab,Vector3.zero, messageBoxPrefab.transform.rotation, ChatScrollBox.gameObject.transform);
            otherChat.image.sprite = otherMessageSprite;
            otherChat.SetText(text);
            otherChat.boxTransform.pivot = Vector2.up;
            otherChat.boxTransform.anchorMin = Vector2.up;
            otherChat.boxTransform.anchorMax = Vector2.up;

            return otherChat;
        }

        public ChatTextBox SpawnAnswerChat(string text)
        {
            var answerChat = Instantiate(messageBoxPrefab,Vector3.zero, messageBoxPrefab.transform.rotation, ChatScrollBox.gameObject.transform);
            answerChat.image.sprite = myMessageSprite;
            answerChat.boxTransform.pivot = Vector2.one;
            answerChat.boxTransform.anchorMin = Vector2.one;
            answerChat.boxTransform.anchorMax = Vector2.one;
            answerChat.SetText(text);
            
            return answerChat;
        }
        
        public void ShowMyAnswerList()
        {
            Sequence UiSeq = DOTween.Sequence();
            
            foreach (AnswerBlock answerBlock in answerList)
            {
                answerBlock.button.interactable = true;
                UiSeq.Append(answerBlock.rectTransform.DOLocalMoveY(answerBlock.rectTransform.localPosition.y - 30f, 0.35f).From()).Join(answerBlock.canvasGroup.DOFade(1f, 0.35f));
            }

            UiSeq.AppendCallback(() => isChatLogContentSizeUpdate = false);
        }

        public void ShowOtherChatBox()
        {
            if(otherChatBox == null) return;
            
            otherChatBox.boxTransform.DOLocalMoveY(otherChatBox.boxTransform.localPosition.y - 50f, 0.5f).From();
            isChatLogContentSizeUpdate = true;
        }
        
        public void ShowAnswerChatBox()
        {
            if(answerChatBox == null) return;
            
            answerChatBox.boxTransform.DOLocalMoveY(answerChatBox.boxTransform.localPosition.y - 50f, 0.5f).From();
            isChatLogContentSizeUpdate = true;
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

        [FormerlySerializedAs("ChatGage")]
        [Space]
        [Header("게이지")]
        public int chatGage;
        public Image gageBarImage;

        [Header("데이터베이스에서 가져올 정보")] 
        [Space]
        private TalkingData talkData = null;
        private TalkingData prevTalkData;
        public MinMaxValue<float> ignoreTimer = new(15, 0, 15);
        public UnityEvent completeEvent;
        public UnityEvent ignoreEvent;

        public RectTransform ChatScrollBox;
        private bool isInit = false;
        public void ConversationClear()
        {
            if (gageBarImage.fillAmount >= 1 || talkData == null || answerList.Count == 0)
            {
                //클리어
                completeEvent?.Invoke();
                isInit = false;
                backButton.gameObject.SetActive(true);
                
                ignoreEvent = new();
                completeEvent = new();
            }
        }

        public void ChoiceButton(AnswerBlock block) //버튼 클릭시
        {
            
            Sequence UiSeq = DOTween.Sequence();
            
            foreach (AnswerBlock answerBlock in answerList)
            {
                answerBlock.button.interactable = false;
                if (answerBlock == block)
                {
                    UiSeq.Append(answerBlock.canvasGroup.DOFade(0f, 0.2f));
                    answerChatBox = SpawnAnswerChat(block.answerText.text);
                    ShowAnswerChatBox();
                }
                else
                {
                    UiSeq.Append(answerBlock.canvasGroup.DOFade(0f, 0.2f));
                }
            }

            ignoreTimer.Current += 3;
            chatGage += block.gage;
            UiSeq.Append(gageBarImage.DOFillAmount(chatGage / 100f, 1f));
            UiSeq.AppendCallback(() =>
            {
                prevTalkData = talkData;
                talkData = TalkingScriptableObject.Instance.GetTalkData(block.isPositive ? talkData.positiveResultTalkID : talkData.negativeResultTalkID);

                if (talkData != null)
                {
                    SettingAnswer(); 
                    otherChatBox = SpawnOtherChat(talkData.mainText);
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
                    script.StartConversation();
                }
            }
        }
    }

#endif
    
}