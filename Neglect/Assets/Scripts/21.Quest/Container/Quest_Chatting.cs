using GamePlay;
using GamePlay.App;
using GamePlay.Chatting;
using GamePlay.Narration;
using GamePlay.Phone;
using GamePlay.Talk;
using UnityEngine;

namespace Quest.Container
{
    public class Quest_Chatting : QuestBase
    {
        private AppButton appButton;

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
                appButton = phone.applicationControl.GetHomeApp().GetAppButton(chatting);
                
                var chatConversation = chatting.conversation;
                chatConversation.Init();
                
                // Name 셋팅
                if (eventData.textArray.Length > 0) chatConversation.ChatName.text = eventData.textArray[0];
                
                // Extra Data 셋팅
                chatConversation.SetTalkData(TalkingScriptableObject.Instance.GetTalkData(eventData.extraDataIDArray.Length == 0 ? -1 : eventData.extraDataIDArray[0]));
                if(eventData.extraDataIDArray.Length > 1) GameManager.Instance.narration.StartNarration(NarrationScriptableObject.Instance.GetNarrationID(eventData.extraDataIDArray[1]));
                chatConversation.completeEvent.AddListener(isReverse ? Ignore : Complete);
                chatConversation.ignoreEvent.AddListener(isReverse ? Complete : Ignore);
                chatConversation.StartConversation();
            }
        }

        public override void Failed()
        {
            base.Failed();
            if(appButton) appButton.button.interactable = false;
        }
    }
}