using System;
using UnityEngine.Serialization;

namespace Quest
{
    [Serializable]
    public class EventData
    {
        public int id;
        public QuestLevel level;
        public QuestBase prefab;
        public EventData acceptEvent;
        public EventData ignoreEvent;
        public string[] textArray;
    }
}