using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Util;

namespace Quest
{
    public class QuestManager : Singleton<QuestManager>
    {
        [HideInInspector] public bool isQuestStart;
        public MinMaxValue<float> questSpawnTimer = new(0, 0, 60,true,true);
        
        private Dictionary<QuestType, Subject<object>> questDictionary = new();

        public void Update()
        {
            if (isQuestStart)
            {
                questSpawnTimer.Current += Time.deltaTime;

                if (questSpawnTimer.IsMax)
                {
                    questSpawnTimer.Current -= questSpawnTimer.Max;
                    isQuestStart = false;
                    var quest = QuestDataList.Instance.InstantiateRandomQuest();
                    quest.Play();
                }
            }
        }

        public void Init()
        {
            questDictionary.Clear();
            isQuestStart = true;
            questSpawnTimer.SetMin();
        }
        
        // 퀘스트를 매니저에 추가
        public IDisposable Add(QuestBase quest)
        {
            if (!questDictionary.TryGetValue(quest.type, out var subject))
            {
                subject = new();
                questDictionary.Add(quest.type, subject);
            }
            return subject.Subscribe(quest.OnNext);
        }
        
        /// <summary>
        /// 퀘스트 진행사항을 업데이트 하기 위한 함수
        /// 다른 클래스에서 호출하여 사용
        /// </summary>
        /// <param name="eventType">진행중인 퀘스트 이벤트</param>
        /// <param name="value">이벤트 값</param>
        public void OnValueChange(QuestType eventType, object value)
        {
            if (questDictionary.TryGetValue(eventType, out var subject))
            {
                subject.OnNext(value);
            }
        }
    }
}