using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public abstract partial class QuestBase : MonoBehaviour
    {
        [Tooltip("퀘스트 ID")] public int questID;
        [Tooltip("퀘스트 제목")] public string questName;
        [Tooltip("퀘스트 난이도")] public QuestLevel level;
        [Tooltip("퀘스트 타입")] public QuestEvent eventType = QuestEvent.None;

        [Space] 
        [Tooltip("퀘스트 진행 상태")] public QuestState state = QuestState.NotStarted;

        public List<QuestPrefab> questPrefabList = new();
        [HideInInspector] public QuestBase nextQuest;
        
        protected IDisposable subscription; // 퀘스트 매니저에서 구독하면 자동 할당됨

        public virtual void Play()
        {
            if (state != QuestState.Completed)
            {
                subscription?.Dispose();
                subscription = QuestManager.Instance.Add(this);
                
                // 퀘스틑 관련 프리펩 스폰
                foreach (QuestPrefab prefab in questPrefabList)
                    Instantiate(prefab, transform);
                
                state = QuestState.InProgress;
            }
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
            if (nextQuest != null) Instantiate(nextQuest).Play();
            OnCompleted();
        }
    }

    public abstract partial class QuestBase : IObserver<object>
    {
        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(object value);
    }
}

