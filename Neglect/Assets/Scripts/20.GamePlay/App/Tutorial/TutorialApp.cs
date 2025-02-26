using GamePlay.Phone;
using UnityEngine;

namespace GamePlay.App.Tutorial
{
    public partial class TutorialApp : MonoBehaviour
    {
        [Header("Tutorial Canvas 관련")] 
        public Canvas tutorialCanvas;
    }

    public partial class TutorialApp : IPhoneApplication
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
            tutorialCanvas.gameObject.SetActive(value);
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            var home = _phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            home.appGridControl.Insert(appButton, new (3,1));

            tutorialCanvas.worldCamera = _phone.phoneCamera;
            
            SetActiveBackground(false);
        }

        public void AppPlay(PhoneControl phone)
        {
            SetActiveBackground(true);
        }

        public void AppResume(PhoneControl phone)
        {
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