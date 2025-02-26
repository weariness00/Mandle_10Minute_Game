using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Quest
{
    public abstract partial class QuestBase : MonoBehaviour
    {
        [Tooltip("퀘스트 ID")] public int questID;
        [Tooltip("퀘스트 제목")] public string questName;
        [Tooltip("퀘스트 타입")] public QuestType type = QuestType.None;

        [Space] 
        [Tooltip("퀘스트 진행 상태")] public QuestState state = QuestState.NotStarted;
        
        [HideInInspector] public EventData eventData;
        [HideInInspector] public bool isReverse; // 성공 실패에 대한 것을 뒤집을 것인지
        [HideInInspector] public bool isLoop = false; // 재활용 가능한 퀘스트인지 ( 해결해도 다시 큐에 돌아가서 또 나타날 수 있다.)
        protected IDisposable subscription; // 퀘스트 매니저에서 구독하면 자동 할당됨

        private QuestBase rootQuest = null;
        public UnityEvent<QuestBase> onCompleteEvent = new();
        public UnityEvent<QuestBase> onIgnoreEvent = new();
        
        public virtual void Play()
        {
            questName = eventData.name;
            if (state != QuestState.InProgress && state != QuestState.Completed)
            {
                subscription?.Dispose();
                subscription = QuestManager.Instance.Add(this);
                
                state = QuestState.InProgress;
            }

            if (rootQuest != null)
            {
                rootQuest.state = state;
                isReverse = rootQuest.isReverse;
            }
        }

        public virtual void Ignore()
        {
            subscription?.Dispose();
            state = isReverse ? QuestState.Completed : QuestState.Failed;
            if (rootQuest != null) rootQuest.state = state;
            if (eventData.ignoreEventID != -1)
            {
                var ignoreEvent = QuestDataList.Instance.GetEventID(eventData.ignoreEventID);
                var quest = QuestDataList.Instance.InstantiateEvent(eventData.ignoreEventID);
                quest.eventData = ignoreEvent;
                quest.rootQuest = rootQuest == null ? this : rootQuest;
                
                if(eventData.ignoreDuration == 0)
                    QuestManager.Instance.AddAndPlay(quest);
                else
                    StartCoroutine(PlayQuestDurationEnumerator(quest, eventData.acceptDuration));
                
                onIgnoreEvent?.Invoke(quest);
            }
            else
            {
                onIgnoreEvent?.Invoke(null);
            }
            
            QuestManager.Instance.Remove(this);
        }

        public virtual void Pause()
        {
            subscription?.Dispose();
            state = QuestState.Wait;
        }

        public virtual void Complete()
        {
            subscription?.Dispose();
            state = isReverse ? QuestState.Failed : QuestState.Completed;
            if (rootQuest != null) rootQuest.state = state;
            if (eventData.acceptEventID != -1)
            {
                var acceptEvent = QuestDataList.Instance.GetEventID(eventData.acceptEventID);
                var quest = QuestDataList.Instance.InstantiateEvent(eventData.acceptEventID);
                quest.eventData = acceptEvent;
                quest.rootQuest = rootQuest == null ? this : rootQuest;
                
                if(eventData.acceptDuration == 0)
                    QuestManager.Instance.AddAndPlay(quest);
                else
                    StartCoroutine(PlayQuestDurationEnumerator(quest, eventData.acceptDuration));
                onCompleteEvent?.Invoke(quest);
            }
            else
            {
                onCompleteEvent?.Invoke(null);
            }
            
            QuestManager.Instance.Remove(this);
        }

        public virtual void Failed()
        {
            subscription?.Dispose();
            state = QuestState.Failed;
            if (rootQuest != null) rootQuest.state = state;
            QuestManager.Instance.Remove(this);
        }

        private IEnumerator PlayQuestDurationEnumerator(QuestBase quest, float duration)
        {
            yield return new WaitForSeconds(duration);
            QuestManager.Instance.AddAndPlay(quest);
        }
    }

    public abstract partial class QuestBase
    {
        // QuestManager를 통해 특수한 값이 전달되었을떄 사용
        public abstract void OnNext(object value);
    }

    public partial class QuestBase : IComparable, IComparable<int>, IComparable<QuestBase>
    {
        public override string ToString()
        {
            return $"[{eventData.level.ToStringEx()}] {questName}";
        }

        public int CompareTo(object obj)
        {
            if (obj is int id)
                return this.questID.CompareTo(id);

            return 0;
        }

        public int Compare(object x, object y)
        {
            if (x is QuestBase quest)
                return quest.CompareTo(y);

            return 0;
        }

        public int CompareTo(QuestBase other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return questID.CompareTo(other.questID);
        }
        
        public int CompareTo(int otherID)
        {
            return questID.CompareTo(otherID);
        }
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(QuestBase), true)]
    public class QuestBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as QuestBase;
            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("난이도 쉬움으로 변경"))
                {
                    script.eventData.level = QuestLevel.Easy;
                }
                    
            }
        }
    }
    
#endif
}

