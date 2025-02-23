using System;

namespace GamePlay.Narration
{
    [Serializable]
    public partial class NarrationData
    {
        public int id = -1;
        public string name = "";
        public string text = "";
        public float stayDuration = 0;
    }
    
    public partial class NarrationData : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is int otherID)
                return id.CompareTo(otherID);
            return 0;
        }
    }
}