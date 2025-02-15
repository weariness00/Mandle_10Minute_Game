using System;
using System.Collections.Generic;

namespace GamePlay.Talk
{
    [Serializable]
    public partial class TalkingData
    {
        public int id;
        public string mainText;
        
        public TalkingData positiveResultTalk;
        public TalkingData negativeResultTalk;
        
        public string[] positiveTextArray;
        public string[] negativeTextArray;
    }
}

