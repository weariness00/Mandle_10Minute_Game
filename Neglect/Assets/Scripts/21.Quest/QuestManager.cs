using GamePlay;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Util;
using Random = UnityEngine.Random;

namespace Quest
{
    public partial class QuestManager : Singleton<QuestManager>
    {
        [HideInInspector] public bool isQuestStart;

        private MinMaxValue<float> questSpawnTimer;
        
        public override void Awake()
        {
            base.Awake();
            questSpawnTimer = QuestDataList.Instance.questSpawnTimer;
        }

        public void Update()
        {
            if (isQuestStart)
            {
                questSpawnTimer.Current += Time.deltaTime;

                if (questSpawnTimer.IsMax)
                {
                    questSpawnTimer.Current -= questSpawnTimer.Max;
                    var quest = InstantiateRandomEvent();
                    AddQuestQueue(quest);
                }
            }
        }
    }

    // 퀘스트 관리 관련
    public partial class QuestManager
    {
        private Dictionary<QuestType, Subject<object>> questPlayDictionary = new(); // 퀘스트가 클리어되면 여기서 제외됨
        [SerializeField] private List<QuestBase> addMainQuestList = new();
        [SerializeField]private List<QuestBase> questAddList = new(); // 추가된 퀘스트들
        [SerializeField]private List<QuestBase> questPlayList = new(); // 플레이 중인 퀘스트들
        [SerializeField]private Queue<QuestBase> waitQuestList = new(); // 대기중인 퀘스트 playQuestList에 있는 퀘스트들이 끝나야 작동함

        [HideInInspector] public UnityEvent<QuestBase> onEndQuestEvent = new(); // 진행중인 퀘스트가 끝났을때
        
        private List<EventData> eventList = new(); // 소환 가능한 이벤트 목록

        public bool IsHasPlayQuest => questPlayList.Count != 0; // 현재 플레이중인 퀘스트가 있는지
        public List<QuestBase> GetAllQuest() => addMainQuestList;
        public List<QuestBase> GetPlayQuestList() => questPlayList;
        
        public void Init()
        {
            questPlayDictionary.Clear();
            addMainQuestList.Clear();
            questAddList.Clear();
            questPlayList.Clear();
            waitQuestList.Clear();
        }

        public void QuestStart()
        {
            isQuestStart = true;
            questSpawnTimer.SetMin();
            eventList = new(QuestDataList.Instance.GetAllMainEvent());
        }

        public void AddAndPlay(QuestBase quest)
        {
            if(quest == null) return;
            
            questAddList.Add(quest);
            if(quest.eventData.isMainEvent) addMainQuestList.Add(quest);
            quest.Play();
        }

        public void AddQuestQueue(QuestBase quest)
        {
            if(quest == null) return;
            
            questAddList.Add(quest);
            if(quest.eventData.isMainEvent) addMainQuestList.Add(quest);
            if (questPlayList.Count == 0)
            {
                quest.Play();
            }
            else
            {
                waitQuestList.Enqueue(quest);
            }
        }
        
        // 퀘스트를 매니저에 추가
        public IDisposable Add(QuestBase quest)
        {
            eventList.Remove(quest.eventData);

            Subject<object> subject = new();
            if (!questPlayDictionary.TryGetValue(quest.type, out subject))
            {
                subject = new();
                questPlayDictionary.Add(quest.type, subject);
            }
            
            questPlayList.Add(quest);
            return subject.Subscribe(quest.OnNext);
        }

        public void Remove(QuestBase quest)
        {
            questPlayList.Remove(quest);
            onEndQuestEvent?.Invoke(quest);
            if (questPlayList.Count == 0 && waitQuestList.Count != 0)
            {
                var nextQuest = waitQuestList.Dequeue();
                if(nextQuest) nextQuest.Play();
            }
            
            if(!quest.isLoop && quest.eventData.isMainEvent)
                eventList.Add(quest.eventData);
        }

        // 모든 퀘스트 다 failed 시키기
        public void AllQuestFailed()
        {
            // 먼저 대기중인거 Failed 호출
            foreach (QuestBase quest in waitQuestList)
                quest.Failed();
            waitQuestList.Clear();

            // 진행중인거 Failed 호출
            List<QuestBase> list = new(questPlayList);
            foreach (QuestBase quest in list)
                quest.Failed();
            questPlayList.Clear();
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
            var quest = QuestDataList.Instance.InstantiateMainEvent(e.id);
            if(quest) quest.eventData = e;
            return quest;
        }
    }
}