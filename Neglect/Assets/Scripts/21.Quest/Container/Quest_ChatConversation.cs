using GamePlay.Chatting;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_ChatConversation : QuestBase
    {
        [Header("소환할 프리팹")]
        public ChatConversation ChatPrefab; 
        
        private ChatConversation chatconversation;
        public override void OnNext(object value)
        {
        }

        public override void Play()
        {
            base.Play();
            chatconversation = PhoneUtil.InstantiateUI(ChatPrefab, out var phone);
            phone.PhoneViewRotate(0);
            chatconversation.ClearAction += Complete;
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