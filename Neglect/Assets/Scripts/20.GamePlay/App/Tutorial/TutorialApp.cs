using GamePlay.Phone;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GamePlay.App.Tutorial
{
    public partial class TutorialApp : MonoBehaviour
    {
        [Header("Tutorial Canvas 관련")] 
        public Canvas tutorialCanvas;
        public GameObject realTutorialImagePrefab; // 실제 튜토리얼 이미지

        [HideInInspector] public GameObject realTutorialObject;
        [HideInInspector] public Button realTutorialButton;
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
            realTutorialObject.SetActive(value);
            tutorialCanvas.gameObject.SetActive(value);
            _phone.isUpdateInteract = !value;
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            var home = _phone.applicationControl.GetHomeApp();
            var appButton = home.GetAppButton(this);
            home.appGridControl.Insert(appButton, new (3,1));
            home.firstStartWindow.clickEntry.callback.AddListener(arg0 =>
            {
                SetActiveBackground(true);
            });
            
            tutorialCanvas.worldCamera = _phone.phoneCamera;
            
            realTutorialObject = Instantiate(realTutorialImagePrefab, GameManager.Instance.worldCanvas.transform);
            realTutorialButton = realTutorialObject.GetComponentInChildren<Button>();
            realTutorialButton.onClick.AddListener(() =>
            {
                phone.applicationControl.CloseApp(this);
            });
            
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