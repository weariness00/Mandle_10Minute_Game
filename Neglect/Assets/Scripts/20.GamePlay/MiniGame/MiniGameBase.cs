﻿using GamePlay.App;
using GamePlay.Phone;
using Manager;
using Quest;
using Quest.Container;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame
{
    public partial class MiniGameBase : MonoBehaviour
    {
        [Header("공통 사항")]
        public ReactiveProperty<bool> isGameStart = new(false);
        public ReactiveProperty<bool> isGamePlay = new(true);
        public ReactiveProperty<bool> isGameClear = new(false);
        public ReactiveProperty<float> gameSpeed = new(1f);
        [SerializeField] protected MinMaxValue<float> playTime = new(0, 0, 60 * 10);

        public MiniGameTutorial tutorial;
        [HideInInspector] public bool isOnTutorial;

        [Tooltip("랭크 퀘스트")] public QuestBase rankQuest;
        
        [Space] 
        public AudioClip bgmSound;
        public virtual void Awake()
        {
            tutorial.gameObject.SetActive(false);
            isGamePlay.Value = false;
            playTime.SetMin();
            if (GameManager.HasInstance)
                playTime = GameManager.Instance.playTimer;
            
            if(PlayerPrefs.HasKey($"{nameof(isOnTutorial)}{AppName}"))
                isOnTutorial = PlayerPrefs.GetInt($"{nameof(isOnTutorial)}{AppName}") == 1;
        }

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            if (isGamePlay.Value && !isGameClear.Value)
            {
                if (!GameManager.HasInstance)
                    playTime.Current += Time.deltaTime;
                if (playTime.IsMax)
                {
                    GameClear();
                }
            }
        }

        public virtual void GamePlay()
        {
            if (isOnTutorial)
            {
                var bgmSource = SoundManager.Instance.GetBGMSource();
                bgmSource.clip = bgmSound;
                bgmSource.Play();
            }
            else
            {
                isOnTutorial = true;
                PlayerPrefs.SetInt($"{nameof(isOnTutorial)}{AppName}", 1);
                tutorial.gameObject.SetActive(true);
                tutorial.okButton.onClick.AddListener(() =>
                {
                    tutorial.gameObject.SetActive(false);
                    GamePlay();
                });
            }
        }

        public virtual void GameStop()
        {
            isGamePlay.Value = false;
            SoundManager.Instance.GetBGMSource().Pause();
        }

        public virtual void GameOver()
        {
            isGamePlay.Value = false;
            gameSpeed.Value = 0;
            if (GameManager.HasInstance) GameManager.Instance.GameClear();
            if (QuestManager.HasInstance) QuestManager.Instance.AddAndPlay(rankQuest);
        }

        public virtual void GameClear()
        {
            if(isGameClear.Value) return;
            
            isGamePlay.Value = false;
            isGameClear.Value = true;
            if (GameManager.HasInstance) GameManager.Instance.GameClear();
            if (QuestManager.HasInstance) QuestManager.Instance.AddAndPlay(rankQuest);
        }

        public virtual void GameExit()
        {
            if(_phone)
                _phone.applicationControl.CloseApp(this);
        }
        

        public void SetGameSpeed(float value)
        {
            gameSpeed.Value = value;
        }

    }

    public partial class MiniGameBase : IPhoneApplication
    {
        [Header("App 관련")]
        [SerializeField] private string gameName;
        [SerializeField] private Sprite appIcon;
        [SerializeField] private Vector2Int resolution;
        [SerializeField] private PhoneControl _phone;
        private PhoneViewPort viewPort;

        public string AppName => gameName;
        public Sprite AppIcon { get => appIcon; set => appIcon = value; }
        public Vector2Int VerticalResolution { get => resolution; set => resolution = value; }
        public PhoneControl Phone => _phone;
        public AppState AppState { get; set; }

        public virtual void SetActiveBackground(bool value)
        {
            
        }
        
        public virtual void AppInstall(PhoneControl phone)
        {
            _phone = phone;
            viewPort = _phone.GetAppViewPort(this);
            foreach (GameObject rootGameObject in gameObject.scene.GetRootGameObjects())
            {
                // 카메라에 따라 마우스 클릭 위치 변경 가능
                foreach (var canvas in rootGameObject.GetComponentsInChildren<Canvas>())
                {
                    canvas.worldCamera = phone.phoneCamera;
                }
            }
            
            var home = _phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            appButton.highlightObject.SetActive(true);
            appButton.button.interactable = false;
            home.appGridControl.Insert(appButton, new(2,2));

            // 러닝 게임 시작시 GameManager의 타이머가 흐르도록 설정
            isGameStart.Subscribe(value =>
            {
                if (value)
                {
                    GameManager.Instance.isGameStart.Value = true;
                }
            });
            
            // 게임 클리어
            GameManager.Instance.isGameClear.Subscribe(value =>
            {
                if (value)
                {
                    var allButtonArray = home.GetAllAppButton();
                    foreach (AppButton button in allButtonArray)
                        button.button.interactable = false;
                    appButton.button.interactable = true;
                    GameClear();
                }
            });
            
            GameManager.Instance.onLastEvent.AddListener(quest =>
            {
                // 은행 어플에 진입하는 퀘스트 까지 가면 게임 앱 버튼 비활성화
                quest.onCompleteEvent.AddListener(chatQuest =>
                {
                    chatQuest.onCompleteEvent.AddListener(bankQuest =>
                    {
                        appButton.button.interactable = false;
                    });
                });
                
                quest.onIgnoreEvent.AddListener(callScreenQuest =>
                {
                    // 은행앱 진입
                    callScreenQuest.onCompleteEvent.AddListener(phoneCallQuest =>
                    {
                        phoneCallQuest.onCompleteEvent.AddListener(bankQuest =>
                        {
                            appButton.button.interactable = false;
                        });
                    });
                });
            });
        }

        public virtual void AppPlay(PhoneControl phone)
        {
            var home = _phone.applicationControl.GetHomeApp();
            home.GetAppButton(this).highlightObject.SetActive(false);
            phone.PhoneViewRotate(PhoneViewType.Horizon);
        }

        public virtual void AppResume(PhoneControl phone)
        {
            phone.FadeOut(0f, Color.black);
            phone.PhoneViewRotate(PhoneViewType.Horizon, () => phone.FadeIn(1f, Color.black));
        }

        public virtual void AppPause(PhoneControl phone)
        {
        }
        
        public virtual void AppExit(PhoneControl phone)
        {
        }

        public virtual void AppUnInstall(PhoneControl phone)
        {
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(MiniGameBase), true)]
    public partial class MiniGameBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as MiniGameBase;

            if (GUILayout.Button("튜토리얼 초기화"))
            {
                PlayerPrefs.SetInt($"{nameof(script.isOnTutorial)}{script.AppName}", 0);
            }
            
            base.OnInspectorGUI();
        }
    }
    
#endif
}