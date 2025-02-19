using GamePlay.Chatting;
using UnityEngine;

namespace GamePlay.Phone
{
    public partial class ChattingApp : MonoBehaviour
    {
        [Header("채팅 관련")] 
        [SerializeField] private Conversation chatting;
    }

    public partial class ChattingApp : IPhoneApplication
    {
        [Header("Phone 관련")]
        [SerializeField] private string appName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int verticalResolution;
        [SerializeField] private PhoneControl _phone;
        
        public string AppName => appName;
        public Sprite AppIcon { get => icon; set => icon =value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public PhoneControl Phone => _phone;

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            chatting.canvas.worldCamera = _phone.phoneCamera;
            
            chatting.gameObject.SetActive(false);
        }

        public void AppPlay(PhoneControl phone)
        {
            chatting.gameObject.SetActive(true);
            chatting.Init();
        }

        public void AppResume(PhoneControl phone)
        {
            chatting.gameObject.SetActive(true);
            chatting.Init();
        }

        public void AppPause(PhoneControl phone)
        {
            chatting.gameObject.SetActive(false);
        }

        public void AppExit(PhoneControl phone)
        {
            chatting.gameObject.SetActive(false);
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

