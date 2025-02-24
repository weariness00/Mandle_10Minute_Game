using GamePlay.Phone;
using Manager;
using Quest;
using UniRx;
using UnityEditor;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame
{
    public partial class MiniGameBase : MonoBehaviour
    {
        [Header("공통 사항")]
        public bool isGameStart = false;
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
                playTime.Current += Time.deltaTime;
                if (playTime.IsMax)
                {
                    isGameClear.Value = true;
                    GameClear();
                }
            }
        }

        public virtual void GamePlay()
        {
            if (isOnTutorial)
            {
                isGamePlay.Value = true;
                isGameStart = true;

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
            QuestManager.Instance.AddAndPlay(rankQuest);
            isGamePlay.Value = false;
            isGameStart = false;
            gameSpeed.Value = 0;
        }

        public virtual void GameClear()
        {
            isGamePlay.Value = false;
            QuestManager.Instance.AddAndPlay(rankQuest);
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
            foreach (GameObject rootGameObject in gameObject.scene.GetRootGameObjects())
            {
                // 카메라에 따라 마우스 클릭 위치 변경 가능
                foreach (var canvas in rootGameObject.GetComponentsInChildren<Canvas>())
                {
                    canvas.worldCamera = phone.phoneCamera;
                }
            }

            GameManager.Instance.isGameClear.Subscribe(value =>
            {
                if (value)
                    GameClear();
            });

            var home = _phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            appButton.highlightObject.SetActive(true);
            appButton.button.interactable = false;
        }

        public virtual void AppPlay(PhoneControl phone)
        {
            var home = _phone.applicationControl.GetHomeApp();
            home.GetAppButton(this).highlightObject.SetActive(false);

            GameManager.Instance.isGameStart.Value = true;
            if (GameManager.Instance.isInitQuest == false)
            {
                GameManager.Instance.isInitQuest = true;
                QuestManager.Instance.Init();
                QuestManager.Instance.QuestStart();
                phone.PhoneViewRotate(PhoneViewType.Horizon);
            }
        }

        public virtual void AppResume(PhoneControl phone)
        {
            phone.PhoneViewRotate(PhoneViewType.Horizon);
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
    public class MiniGameBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as MiniGameBase;

            if (GUILayout.Button("튜토리얼 초기화"))
            {
                PlayerPrefs.SetInt($"{nameof(script.isOnTutorial)}{script.AppName}", 0);
            }
        }
    }
    
#endif
}