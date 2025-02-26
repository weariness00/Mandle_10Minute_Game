using GamePlay.Chatting;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay.Phone
{
    public partial class ChattingApp : MonoBehaviour
    {
        [Header("채팅 관련")] 
        public Conversation conversation;
    }

    public partial class ChattingApp : IPhoneApplication
    {
        [Header("Phone 관련")]
        [SerializeField] private string appName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int verticalResolution;
        [SerializeField] private PhoneControl _phone;
        public AppState AppState { get; set; }

        public string AppName => appName;
        public Sprite AppIcon { get => icon; set => icon =value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public PhoneControl Phone => _phone;

        public void SetActiveBackground(bool value)
        {
            conversation.gameObject.SetActive(value);
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            conversation.canvas.worldCamera = _phone.phoneCamera;
            conversation.backButton.onClick.AddListener(() => phone.applicationControl.CloseApp(this));
            
            SetActiveBackground(false);
        }

        public void AppPlay(PhoneControl phone)
        {
            SetActiveBackground(true);
            conversation.StartConversation();
            phone.PhoneViewRotate(PhoneViewType.Vertical, () => phone.FadeIn(1f, Color.black));
        }

        public void AppResume(PhoneControl phone)
        {
            SetActiveBackground(true);
            conversation.StartConversation();
            phone.FadeOut(0.1f, Color.black);
            phone.PhoneViewRotate(PhoneViewType.Vertical, () => phone.FadeIn(1f, Color.black));
        }

        public void AppPause(PhoneControl phone)
        {
            SetActiveBackground(false);
        }

        public void AppExit(PhoneControl phone)
        {
            SetActiveBackground(false);
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

