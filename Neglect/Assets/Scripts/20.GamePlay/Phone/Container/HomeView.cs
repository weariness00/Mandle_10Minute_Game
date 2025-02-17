using DG.Tweening;
using System;
using UniRx;
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

        [Header("Interface Button")] 
        public RectTransform interfaceRectTransform;
        public Button homeButton;
        public Button backButton;

        private Vector2 interfaceOriginAnchorsPosition;

        public void Awake()
        {
            interfaceOriginAnchorsPosition = interfaceRectTransform.anchoredPosition;
        }

        public void InterfaceOnOff()
        {
            if (!interfaceRectTransform.gameObject.activeSelf)
            {
                interfaceRectTransform.gameObject.SetActive(true);
                interfaceRectTransform.DOAnchorPosY(interfaceOriginAnchorsPosition.y, 0.7f).SetEase(Ease.Flash);
            }
            else
            {
                interfaceRectTransform.DOAnchorPosY(-interfaceRectTransform.sizeDelta.y, 0.7f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    interfaceRectTransform.gameObject.SetActive(false);
                });
            }
        }
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
            
            _phone.interfaceGroupOnOffButton.onClickEvent.AddListener(() =>
            {
                if(_phone.applicationControl.currentPlayApplication != this as IPhoneApplication)
                    InterfaceOnOff();
            });
            
            homeButton.onClick.AddListener(() =>
            {
                _phone.applicationControl.OnHome();
            });
            
            backButton.onClick.AddListener(() =>
            {
                _phone.applicationControl.CloseApp();
            });
        }

        public void AppPlay(PhoneControl phone)
        {
            // 홈에 버튼 생성
            phone.applicationControl.OnAddAppEvent.AddListener(app =>
            {
                var appButton = Instantiate(appButtonPrefab, appButtonParent);
                if (app.AppIcon) appButton.image.sprite = app.AppIcon;
                appButton.onClick.AddListener(() => phone.applicationControl.OpenApp(app));
                appButton.gameObject.layer = LayerMask.NameToLayer("Phone");
            });
            
            var viewPort = phone.GetAppViewPort(this);
            viewPort.horizon.Release();
            viewPort.horizon = viewPort.vertical;
            phone.PhoneViewRotate(PhoneViewType.Vertical);
        }

        public void AppResume(PhoneControl phone)
        {
            mainCanvas.gameObject.SetActive(true);
            phone.PhoneViewRotate(PhoneViewType.Vertical);
            
            interfaceRectTransform.gameObject.SetActive(true);
            interfaceRectTransform.anchoredPosition = interfaceOriginAnchorsPosition;
        }

        public void AppPause(PhoneControl phone)
        {
            Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
            {
                mainCanvas.gameObject.SetActive(false);
            });

            interfaceRectTransform.gameObject.SetActive(false);
            interfaceRectTransform.anchoredPosition = new(0,-interfaceRectTransform.sizeDelta.y);
        }
        
        public void AppExit(PhoneControl phone)
        {
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

