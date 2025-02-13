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
        [SerializeField] private PhoneControl _phone;
        
        public string AppName { get => appName; }
        public Sprite AppIcon { get => icon; set => icon =value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public PhoneControl Phone => _phone;

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;
            mainCanvas.worldCamera = phone.phoneCamera;
            uiCanvas.worldCamera = phone.phoneCamera;
        }

        public void AppPlay(PhoneControl phone)
        {
            // 홈에 버튼 생성
            phone.applicationControl.OnAddAppEvent.AddListener(app =>
            {
                var appButton = Instantiate(appButtonPrefab, appButtonParent);
                if (app.AppIcon) appButton.image.sprite = app.AppIcon;
                appButton.onClick.AddListener(() => phone.applicationControl.OnApp(app));
                appButton.gameObject.layer = LayerMask.NameToLayer("Phone");
            });
            
            var viewPort = phone.GetAppViewPort(this);
            viewPort.horizon.Release();
            viewPort.horizon = viewPort.vertical;
            viewPort.SetActive(PhoneViewType.Vertical);
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

