using System;

namespace Quest
{
    [Serializable]
    public partial class EventData
    {
        public int id;
        public QuestLevel level;
        public QuestBase prefab;
        public int acceptEventID;
        public int ignoreEventID;
        public string[] textArray;
        public int[] extraDataIDArray;
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