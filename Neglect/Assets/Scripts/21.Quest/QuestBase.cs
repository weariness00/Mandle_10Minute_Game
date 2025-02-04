using System;
using UnityEngine;

namespace Quest
{
    public abstract partial class QuestBase : MonoBehaviour
    {
        [Tooltip("퀘스트 제목")] public string questTitle;
        [Tooltip("퀘스트 타입")] public QuestEvent eventType = QuestEvent.None;

        [Space] 
        [Tooltip("퀘스트 등급")] public QuestType type = QuestType.Normal;
        [Tooltip("퀘스트 진행 상태")]public QuestState state = QuestState.NotStarted;

        protected IDisposable subscription; // 퀘스트 매니저에서 구독하면 자동 할당됨

        public virtual void Play()
        {
            subscription?.Dispose();
            subscription = QuestManager.Instance.Add(this);
            state = QuestState.InProgress;
        }

        public virtual void Stop()
        {
            subscription?.Dispose();
            state = QuestState.Failed;
        }

        public virtual void Pause()
        {
            subscription?.Dispose();
            state = QuestState.Wait;
        }

        public virtual void Complete()
        {
            subscription?.Dispose();
            OnCompleted();
        }
    }

    public abstract partial class QuestBase : IObserver<object>
    {
        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(object value);
    }

    public enum QuestType
    {
        Normal, // 일반
        Hidden, // 히든
    }
}

