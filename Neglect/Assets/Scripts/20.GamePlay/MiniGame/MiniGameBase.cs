using GamePlay.Phone;
using Quest;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GamePlay.MiniGame
{
    public partial class MiniGameBase : MonoBehaviour
    {
        public ReactiveProperty<bool> isGamePlay = new(true);
        public bool isGameStart = false;
        public ReactiveProperty<float> gameSpeed = new(1f);
        [SerializeField] protected MinMaxValue<float> playTime = new(0,0, 60 * 10);

        public virtual void Awake()
        {
            playTime.SetMin();
        }

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            if (isGamePlay.Value)
            {
                playTime.Current += Time.deltaTime;
            }
        }

        public virtual void GamePlay()
        {
            isGamePlay.Value = true;
            isGameStart = true;
        }

        public virtual void GameStop()
        {
            isGamePlay.Value = false;
        }

        public virtual void GameOver()
        {
            isGamePlay.Value = false;
            isGameStart = false;
            gameSpeed.Value = 0;
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
        }

        public virtual void AppPlay(PhoneControl phone)
        {
            if (GameManager.Instance.isInitQuest == false)
            {
                GameManager.Instance.isInitQuest = true;
                QuestManager.Instance.Init();
            }
        }

        public virtual void AppResume(PhoneControl phone)
        {
        }

        public virtual void AppPause(PhoneControl phone)
        {
        }

        public virtual void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

