using GamePlay.Chatting;
using GamePlay.Phone;
using GamePlay.Talk;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_Chatting : QuestBase
    {
        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            var phone = PhoneUtil.currentPhone;
            var chatting = phone.applicationControl.GetApp<ChattingApp>();
            if (chatting)
            {
                var chatConversation = chatting.conversation;
                if (eventData.textArray.Length > 0) chatConversation.ChatName.text = eventData.textArray[0];
                chatConversation.talkData = TalkingScriptableObject.Instance.GetTalkData(eventData.extraDataIDArray.Length == 0 ? -1 : eventData.extraDataIDArray[0]);
                chatConversation.completeEvent.AddListener(Complete);
                chatConversation.ignoreEvent.AddListener(Ignore);
            }
        }
    }
}