using GamePlay.App.Home;
using GamePlay.Phone;
using UnityEngine;

namespace GamePlay.App.Dummy
{
    public partial class DummyApp : MonoBehaviour
    {
        [Header("App Button 관련")] 
        public AppGridControl.CellData cellData;
        [SerializeField] public Vector2Int appGridPosition;
        [SerializeField] public Vector2Int appCellSize = Vector2Int.one;

    }

    public partial class DummyApp : IPhoneApplication
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
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            var home = _phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            home.appGridControl.Insert(appButton, appGridPosition, appCellSize);
            
            appButton.button.onClick.RemoveAllListeners();
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

