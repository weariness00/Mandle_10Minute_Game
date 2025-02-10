using System;
using System.Collections;
using UnityEngine.Serialization;

namespace Quest
{
    [Serializable]
    public partial class EventData
    {
        public int id;
        public QuestLevel level;
        public QuestBase prefab;
        public EventData acceptEvent;
        public EventData ignoreEvent;
        public string[] textArray;
    }

    public partial class EventData : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is int otherID)
                return id.CompareTo(otherID);
            return 0;
        }
    }
}