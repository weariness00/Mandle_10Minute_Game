using GamePlay.Phone;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GamePlay.MiniGame
{
    public partial class MiniGameBase : MonoBehaviour
    {
        public ReactiveProperty<bool> isGamePlay = new(true);
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
        }

        public virtual void GameStop()
        {
            isGamePlay.Value = false;
        }

        public virtual void GameOver()
        {
            isGamePlay.Value = false;
            gameSpeed.Value = 0;
        }

        public void InitLoadedScene(Scene scene)
        {
            var phone = FindObjectOfType<PhoneControl>();
            
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                // 미니 게임 씬에 있는 모든 객체는 Phone 레이어를 가지도록 변경
                foreach (Transform t in rootGameObject.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Phone");
                }

                // 카메라에 따라 마우스 클릭 위치 변경 가능
                foreach (var canvas in rootGameObject.GetComponentsInChildren<Canvas>())
                {
                    canvas.worldCamera = phone.phoneCamera;
                }
            }
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

        public string AppName => gameName;
        public Sprite AppIcon { get => appIcon; set => appIcon = value; }
        public Vector2Int VerticalResolution { get => resolution; set => resolution = value; }

        public virtual void AppInstall()
        {
            InitLoadedScene(gameObject.scene);
        }

        public virtual void AppPlay()
        {
        }

        public virtual void AppResume()
        {
        }

        public virtual void AppPause()
        {
        }

        public virtual void AppUnInstall()
        {
        }
    }
}

