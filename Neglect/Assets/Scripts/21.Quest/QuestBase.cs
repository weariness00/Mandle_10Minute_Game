using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        protected IDisposable subscription; // 퀘스트 매니저에서 구독하면 자동 할당됨

        public virtual void Play()
        {
            if (state != QuestState.Completed)
            {
                subscription?.Dispose();
                subscription = QuestManager.Instance.Add(this);
                
                state = QuestState.InProgress;
            }
        }

        public virtual void Ignore()
        {
            subscription?.Dispose();
            state = QuestState.Failed;

            if (eventData.acceptEvent != null)
            {
                var quest = QuestDataList.Instance.InstantiateQuest(eventData.acceptEvent.id);
                quest.eventData = eventData.acceptEvent;
            }
            else
            {
                QuestManager.Instance.isQuestStart = true;
            }
        }

        public virtual void Pause()
        {
            subscription?.Dispose();
            state = QuestState.Wait;
        }

        public virtual void Complete()
        {
            subscription?.Dispose();
            state = QuestState.Completed;

            if (eventData.ignoreEvent != null)
            {
                var quest = QuestDataList.Instance.InstantiateQuest(eventData.ignoreEvent.id);
                quest.eventData = eventData.ignoreEvent;
            }
            else
            {
                QuestManager.Instance.isQuestStart = true;
            }
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
            return $"{nameof(eventData.level)} {questName} : {nameof(state)}";
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
}

