using GamePlay.Phone;
using Manager;
using UnityEngine;
using UnityEngine.Audio;

namespace GamePlay.App.Setting
{
    public partial class SettingApp : MonoBehaviour
    {
        public Canvas mainCanvas;
        
        public SoundBlock masterVolume;
        public SoundBlock bgmVolume;
        public SoundBlock effectVolume;

        public void Awake()
        {
            foreach (AudioMixerGroup group in SoundManager.Instance.mixer.FindMatchingGroups(""))
            {
                if(group.name == "Master")
                    masterVolume.Initialize(group);
                else if(group.name == "BGM")
                    bgmVolume.Initialize(group);
                else if(group.name == "Effect")
                    effectVolume.Initialize(group);
            }
        }
    }
    
    public partial class SettingApp : IPhoneApplication
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
            mainCanvas.gameObject.SetActive(value);
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            mainCanvas.worldCamera = phone.phoneCamera;

            var home = phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            home.appGridControl.Insert(appButton, new(2,1));

            SetActiveBackground(false);
        }

        public void AppPlay(PhoneControl phone)
        {
            var home = phone.applicationControl.GetHomeApp();
            home.InterfaceActive(true);
            SetActiveBackground(true);
        }

        public void AppResume(PhoneControl phone)
        {
            var home = phone.applicationControl.GetHomeApp();
            home.InterfaceActive(true);
            SetActiveBackground(true);
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

