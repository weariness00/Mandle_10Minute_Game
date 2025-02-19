using GamePlay.Chatting;
using GamePlay.Talk;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_Chatting : QuestBase
    {
        private Conversation chatconversation;
        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            chatconversation = FindObjectOfType<Conversation>(true);
            chatconversation.talkData = TalkingScriptableObject.Instance.GetTalkData(eventData.extraDataIDArray.Length == 0 ? -1 : eventData.extraDataIDArray[0]);
            chatconversation.completeEvent.AddListener(Complete);
            chatconversation.ignoreEvent.AddListener(Ignore);
        }
    }
}