using System;
using System.Collections.Generic;
using UniRx;
using Util;

namespace Quest
{
    public class QuestManager : Singleton<QuestManager>
    {
        private Dictionary<QuestEvent, Subject<object>> questDictionary = new();

        // 퀘스트를 매니저에 추가
        public IDisposable Add(QuestBase quest)
        {
            if (!questDictionary.TryGetValue(quest.eventType, out var subject))
            {
                subject = new();
                questDictionary.Add(quest.eventType, subject);
            }
            return subject.Subscribe(quest);
        }
        
        /// <summary>
        /// 퀘스트 진행사항을 업데이트 하기 위한 함수
        /// 다른 클래스에서 호출하여 사용
        /// </summary>
        /// <param name="eventType">진행중인 퀘스트 이벤트</param>
        /// <param name="value">이벤트 값</param>
        public void OnValueChange(QuestEvent eventType, object value)
        {
            if (questDictionary.TryGetValue(eventType, out var subject))
            {
                subject.OnNext(value);
            }
        }
    }

    public enum QuestEvent
    {
        None,
        GameRank, // 게임 랭크
    }
}