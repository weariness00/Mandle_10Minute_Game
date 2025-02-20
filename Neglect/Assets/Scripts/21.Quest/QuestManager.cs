using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Quest
{
    public partial class QuestManager : Singleton<QuestManager>
    {
        [HideInInspector] public bool isQuestStart;

        private MinMaxValue<float> questSpawnTimer;
        
        public void Awake()
        {
            questSpawnTimer = QuestDataList.Instance.questSpawnTimer;
        }

        public void Update()
        {
            if (isQuestStart)
            {
                questSpawnTimer.Current += Time.deltaTime;

                if (questSpawnTimer.IsMax && playQuestList.Count == 0)
                {
                    questSpawnTimer.Current -= questSpawnTimer.Max;
                    var quest = InstantiateRandomEvent();
                    quest.Play();
                }
            }
        }
        
    }

    // 퀘스트 관리 관련
    public partial class QuestManager
    {
        private Dictionary<QuestType, Subject<object>> questPlayDictionary = new(); // 퀘스트가 클리어되면 여기서 제외됨
        private List<QuestBase> questAddList = new(); // 추가된 퀘스트들
        private List<QuestBase> playQuestList = new(); // 플레이 중인 퀘스트들
        
        private List<EventData> eventList; // 소환 가능한 이벤트 목록
        public List<QuestBase> GetAllQuest() => questAddList;
        
        public void Init()
        {
            questAddList.Clear();
            questPlayDictionary.Clear();
            isQuestStart = true;
            questSpawnTimer.SetMin();
            eventList = new(QuestDataList.Instance.GetAllEvent());
        }
        
        // 퀘스트를 매니저에 추가
        public IDisposable Add(QuestBase quest)
        {
            eventList.Remove(quest.eventData);
            questAddList.Add(quest);
            if (!questPlayDictionary.TryGetValue(quest.type, out var subject))
            {
                subject = new();
                questPlayDictionary.Add(quest.type, subject);
            }
            
            playQuestList.Add(quest);
            return subject.Subscribe(quest.OnNext);
        }

        public void Remove(QuestBase quest)
        {
            playQuestList.Remove(quest);
        }
        
        /// <summary>
        /// 퀘스트 진행사항을 업데이트 하기 위한 함수
        /// 다른 클래스에서 호출하여 사용
        /// </summary>
        /// <param name="eventType">진행중인 퀘스트 이벤트</param>
        /// <param name="value">이벤트 값</param>
        public void OnValueChange(QuestType eventType, object value)
        {
            if (questPlayDictionary.TryGetValue(eventType, out var subject))
            {
                subject.OnNext(value);
            }
        }

        public QuestBase InstantiateRandomEvent()
        {
            int index = Random.Range(0, eventList.Count);
            var e = eventList[index];
            var quest = QuestDataList.Instance.InstantiateEvent(e.id);
            quest.eventData = e;
            return quest;
        }
    }
}