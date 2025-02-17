using DG.Tweening;
using GamePlay.Talk;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Util;


namespace GamePlay.Chatting
{
    public class ChatConversation : MonoBehaviour
    {
        // Start is called before the first frame update

        public Canvas canvas;

        public string CurrentName = "npc";
        public TextMeshProUGUI ChatName;

        [Header("복사할 메시지")]
        public ChatTextBox OtherMessages;
        public ChatTextBox MyMessages;

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
        [HideInInspector] public TalkingData talkData = null;
        
        public Action clearAction;

        public float NextTextPosY;
        public Scrollbar Scrollbar;
        public RectTransform ChatScrollBox;
        private bool isInit = false;

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
            if(talkData == null) return;
            if(isInit) return;
            isInit = true;
            SettingAnswer(); 
            OtherChatSpawn(talkData.mainText);
            ChatBox();
        }

        public void SettingAnswer()
        {
            if (talkData == null || ReferenceEquals(talkData, null))
            {
                Debug.LogError("talkData is null");
                return; // talkData가 null이면 이 함수의 실행을 중단
            }

            // pool 비우기
            foreach (AnswerBlock answerBlock in answerList)
                answerBlockPool.Release(answerBlock);
            answerList.Clear();
            
            // 답변 셋팅
            foreach (string text in talkData.positiveTextArray)
            {
                var answer = answerBlockPool.Get();
                answer.answerText.text = text;
                answer.gage = 20;
                answerList.Add(answer);
            }
            foreach (string text in talkData.negativeTextArray)
            {
                var answer = answerBlockPool.Get();
                answer.answerText.text = text;
                answer.gage = 0;
                answerList.Add(answer);
            }
            
            // 위치 셔플
            answerList.Shuffle();
            for (var i = 0; i < answerList.Count; i++)
                answerList[i].transform.SetSiblingIndex(i);
        }

        public void ChatBox()
        {
            Sequence UiSeq = DOTween.Sequence();
            
            foreach (AnswerBlock answerBlock in answerList)
            {
                answerBlock.button.interactable = true;
                UiSeq.Append(answerBlock.rectTransform.DOLocalMoveY(answerBlock.rectTransform.localPosition.y - 30f, 0.5f).From()).Join(answerBlock.canvasGroup.DOFade(1f, 0.5f));
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
                    UiSeq.Append(answerBlock.canvasGroup.DOFade(0f, 0.5f));
                    MyChatSpawn(block.answerText.text);  // ~버튼 종료 애니메이션
                }
                else
                {
                    UiSeq.Append(answerBlock.canvasGroup.DOFade(0f, 0.5f));
                }
            }

            ChatGage += block.gage;
            UiSeq.Append(GageBar.DOFillAmount(ChatGage / 100f, 1f));

            UiSeq.AppendCallback(() =>
            {
                if (ChatGage == 100)
                {
                    //클리어
                    clearAction.Invoke();
                    clearAction = null;
                    isInit = false;
                }
                else
                {
                    CallStart();
                }
            });

        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ChatConversation))]
    public class ChatConversationEditor : Editor
    {
        private int talkID;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as ChatConversation;
            
            talkID = EditorGUILayout.IntField("TalkID", talkID);
            if (GUILayout.Button("Talk 셋팅"))
            {
                script.talkData = TalkingScriptableObject.Instance.GetTalkData(talkID);
            }

            if (GUILayout.Button("대화 생성"))
            {
                script.CallStart();
            }
        }
    }

#endif
    
}