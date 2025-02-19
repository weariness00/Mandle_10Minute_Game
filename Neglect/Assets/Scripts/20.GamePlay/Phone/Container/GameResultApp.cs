﻿using Quest;
using Quest.UI;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Phone.Container
{
    public partial class GameResultApp : MonoBehaviour
    {
        [Header("Quest 결과 관련")] 
        public QuestResult questResult;
    }

    public partial class GameResultApp : IPhoneApplication
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
            questResult.mainCanvas.worldCamera = _phone.phoneCamera;
            
            questResult.nextButton.onClick.AddListener(() => AppExit(_phone));
        }

        public void AppPlay(PhoneControl phone)
        {
            questResult.Init();
            _phone.PhoneViewRotate(PhoneViewType.Vertical);
        }

        public void AppResume(PhoneControl phone)
        {
        }

        public void AppPause(PhoneControl phone)
        {
        }

        public void AppExit(PhoneControl phone)
        {
            SceneUtil.LoadReal();
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
    
#if UNITY_EDITOR

    [CustomEditor(typeof(GameResultApp))]
    public class GameResultAppEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as GameResultApp;

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("퀘스트 결과 창 생성"))
                {
                    script.questResult.Init();
                }
                
                if (GUILayout.Button("랜덤 퀘스트 클리어 상태로 생성"))
                {
                    var quest = QuestDataList.Instance.InstantiateRandomEvent();
                    QuestManager.Instance.Add(quest);
                    quest.state = QuestState.InProgress;
                }
                if (GUILayout.Button("랜덤 퀘스트 무시 상태로 생성"))
                {
                    var quest = QuestDataList.Instance.InstantiateRandomEvent();
                    QuestManager.Instance.Add(quest);
                    quest.state = QuestState.Completed;
                }
            }
        }
    }
#endif
}

