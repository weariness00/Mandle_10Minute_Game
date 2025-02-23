using DG.Tweening;
using GamePlay.App;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Util;

namespace GamePlay.Phone
{
    public partial class HomeView : MonoBehaviour
    {
        [Header("Canvas")]
        public Canvas mainCanvas;
        public Canvas extraUICanvas;
        public Canvas phoneUIControlCanvas;
        
        [Header("App Button 관련")]
        [SerializeField] private AppButton appButtonPrefab;
        [SerializeField] private Transform appButtonParent;
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private MinMaxValue<float> shineTimer = new (0,0,1, false, true);

        private Dictionary<string, AppButton> appButtonDictionary = new();
        
        private static readonly int ShineLocation = Shader.PropertyToID("_ShineLocation");

        [Header("화면 관리")] 
        [Tooltip("처음 게임 킬때 나오는 화면")]public FirstStartWindow firstStartWindow;
        [Tooltip("앱 키면 나오는 bgm")] public AudioClip bgmSound;
        
        [Header("Informaton Canvas 관련")] 
        public Canvas informationCanvas;
        
        [Header("Interface Button")] 
        public RectTransform interfaceRectTransform;
        public Button homeButton;
        public Button backButton;
        public List<Action> onClickBackList = new();

        private bool isOnInterface = true;
        private Tween tween;
        private Vector2 interfaceOriginAnchorsPosition;

        public void Awake()
        {
            interfaceOriginAnchorsPosition = interfaceRectTransform.anchoredPosition;

            shineTimer.isOverMax = true;
            for (int i = 0; i < appButtonParent.childCount; i++)
            {
                Destroy(appButtonParent.GetChild(i).gameObject);
            }
        }

        public void Update()
        {
            UpdateHighlight();
        }

        public void InterfaceOnOff()
        {
            if (!isOnInterface)
            {
                interfaceRectTransform.gameObject.SetActive(true);
                tween.Kill();
                tween = interfaceRectTransform.DOAnchorPosY(interfaceOriginAnchorsPosition.y, 0.7f).SetEase(Ease.Flash);
            }
            else
            {
                tween.Kill();
                tween = interfaceRectTransform.DOAnchorPosY(-interfaceRectTransform.sizeDelta.y, 0.7f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    interfaceRectTransform.gameObject.SetActive(false);
                });
            }

            isOnInterface = !isOnInterface;
        }
        
        public void UpdateHighlight()
        {
            shineTimer.Current += Time.deltaTime;
            float value = Mathf.Sin((shineTimer.Current % shineTimer.Max) * Mathf.PI); // 0~1~0
            highlightMaterial.SetFloat(ShineLocation, value);
        }

        public AppButton GetAppButton(IPhoneApplication app)
        {
            if (app == null) return null;
            appButtonDictionary.TryGetValue(app.AppName, out var appButton);
            return appButton;
        }
    }

    public partial class HomeView : IPhoneApplication
    {
        [Header("Phone 관련")]
        [SerializeField] private string appName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int verticalResolution;
        [SerializeField] private PhoneControl _phone;
        public AppState AppState { get; set; }

        public string AppName { get => appName; }
        public Sprite AppIcon { get => icon; set => icon =value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public PhoneControl Phone => _phone;

        public void SetActiveBackground(bool value)
        {
            mainCanvas.gameObject.SetActive(value);
            informationCanvas.gameObject.SetActive(value);
            phoneUIControlCanvas.gameObject.SetActive(value);
        }
        
        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;
            mainCanvas.worldCamera = phone.phoneCamera;
            extraUICanvas.worldCamera = phone.phoneCamera;
            phoneUIControlCanvas.worldCamera = phone.phoneCamera;
            
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
                if(onClickBackList.Count == 0)
                    _phone.applicationControl.CloseApp();
                else
                {
                    onClickBackList.Last().Invoke();
                    onClickBackList.RemoveAt(onClickBackList.Count - 1);
                }
            });
        }

        public void AppPlay(PhoneControl phone)
        {
            // 홈에 버튼 생성
            phone.applicationControl.OnAddAppEvent.AddListener(app =>
            {
                var appButton = Instantiate(appButtonPrefab, appButtonParent);
                if (app.AppIcon) appButton.button.image.sprite = app.AppIcon;
                appButton.button.onClick.AddListener(() => phone.applicationControl.OpenApp(app));
                appButton.gameObject.layer = LayerMask.NameToLayer("Phone");

                appButtonDictionary.TryAdd(app.AppName, appButton);
            });
            
            var viewPort = phone.GetAppViewPort(this);
            viewPort.horizon.Release();
            viewPort.horizon = viewPort.vertical;
            phone.PhoneViewRotate(PhoneViewType.Vertical);

            var bgmSource = SoundManager.Instance.GetBGMSource();
            bgmSource.clip = bgmSound;
            bgmSource.Play();
        }

        public void AppResume(PhoneControl phone)
        {
            tween.Kill();
            mainCanvas.gameObject.SetActive(true);
            phone.PhoneViewRotate(PhoneViewType.Vertical);
            
            interfaceRectTransform.gameObject.SetActive(true);
            interfaceRectTransform.anchoredPosition = interfaceOriginAnchorsPosition;
            
            var bgmSource = SoundManager.Instance.GetBGMSource();
            if (bgmSource.clip != bgmSound && bgmSource.isPlaying == false)
            {
                bgmSource.clip = bgmSound;
                bgmSource.Play();
            }
        }

        public void AppPause(PhoneControl phone)
        {
            tween?.Kill();
            // Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
            // {
            //     mainCanvas.gameObject.SetActive(false);
            // });

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

