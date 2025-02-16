using UnityEngine;

namespace GamePlay.Phone
{
    public partial class SettingApp : MonoBehaviour
    {
        
    }

    public partial class SettingApp : IPhoneApplication
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
        }

        public void AppPlay(PhoneControl phone)
        {
        }

        public void AppResume(PhoneControl phone)
        {
        }

        public void AppPause(PhoneControl phone)
        {
        }

        public void AppExit(PhoneControl phone)
        {
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

