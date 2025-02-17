using System;
using System.Collections.Generic;

namespace GamePlay.Talk
{
    [Serializable]
    public partial class TalkingData
    {
        public int id;
        public string mainText; // 질문
        
        public TalkingData positiveResultTalk; // 긍정적 답변시 부를 Talk Data
        public TalkingData negativeResultTalk; // 부정적 답변시 부를 Talk Data
        
        public string[] positiveTextArray; // 긍정적 답변
        public string[] negativeTextArray; // 부정적 답변
    }
    
    public partial class TalkingData : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is int otherID)
                return id.CompareTo(otherID);
            return 0;
        }
    }
}

