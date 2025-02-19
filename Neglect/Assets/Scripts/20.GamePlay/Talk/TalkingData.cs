using System;
using System.Collections.Generic;

namespace GamePlay.Talk
{
    [Serializable]
    public partial class TalkingData
    {
        public int id;
        public string mainText; // 질문

        public int positiveResultTalkID;
        public int negativeResultTalkID;
        
        public string[] positiveTextArray; // 긍정적 답변
        public string[] negativeTextArray; // 부정적 답변

        public int positiveScore; // 긍정적 답변의 점수
        public int negativeScore; // 부정적 답변의 점수
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

