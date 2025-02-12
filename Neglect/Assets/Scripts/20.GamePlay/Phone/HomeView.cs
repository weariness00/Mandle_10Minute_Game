using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Phone
{
    public partial class HomeView : MonoBehaviour
    {
        public Canvas mainCanvas;
        public Canvas uiCanvas;

        [SerializeField] private Button appButtonPrefab;
        [SerializeField] private Transform appButtonParent;
    }

    public partial class HomeView : IPhoneApplication
    {
        [Header("Phone 관련")]
        [SerializeField] private string appName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int verticalResolution;
        
        public string AppName { get => appName; }
        public Sprite AppIcon { get => icon; set => icon =value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public void AppInstall(PhoneControl phone)
        {
            mainCanvas.worldCamera = phone.phoneCamera;
            uiCanvas.worldCamera = phone.phoneCamera;
        }

        public void AppPlay(PhoneControl phone)
        {
            phone.applicationControl.OnAddAppEvent.AddListener(app =>
            {
                var appButton = Instantiate(appButtonPrefab, appButtonParent);
                if (app.AppIcon) appButton.image.sprite = app.AppIcon;
                appButton.onClick.AddListener(() => phone.applicationControl.OnApp(app)); // 앱 중단하고 다시 시작할시에 바꿔야함 이건 테스트용
                appButton.gameObject.layer = LayerMask.NameToLayer("Phone");
            });
        }

        public void AppResume(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(true);
        }

        public void AppPause(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(false);
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

