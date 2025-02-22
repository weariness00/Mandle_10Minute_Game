using System;

namespace Quest
{
    [Serializable]
    public partial class EventData
    {
        public int id = -1;
        public QuestLevel level;
        public QuestBase prefab = null;
        public int acceptEventID = -1;
        public int ignoreEventID = -1;
        public string[] textArray = Array.Empty<string>();
        public int[] extraDataIDArray = Array.Empty<int>();
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