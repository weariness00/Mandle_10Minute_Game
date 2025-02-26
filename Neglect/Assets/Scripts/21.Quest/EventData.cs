using System;

namespace Quest
{
    [Serializable]
    public partial class EventData
    {
        public int id = -1;
        public string name;
        public bool isMainEvent;
        public QuestLevel level;
        public QuestBase prefab = null;
        public int acceptEventID = -1;
        public float acceptDuration = 0;
        public int ignoreEventID = -1;
        public float ignoreDuration = 0;
        public int playNarrationID = -1;
        public int completeNarrationID = -1;
        public int ignoreNarrationID = -1;
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