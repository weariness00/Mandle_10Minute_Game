using GamePlay.Chatting;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_Chatting : QuestBase
    {
        private ChatConversation chatconversation;
        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            chatconversation = FindObjectOfType<ChatConversation>(true);
            chatconversation.clearAction += Complete;
        }

        public override void Complete()
        {
            base.Complete();
        }

        public override void Ignore()
        {
            base.Ignore();
        }
    }
}