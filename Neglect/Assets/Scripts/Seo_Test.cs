using GamePlay;
using GamePlay.MiniGame.FlappingGame;
using GamePlay.Phone;
using Quest;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SeoTestTestTest
{
    public class Seo_Test : MonoBehaviour
    {
        public int questId;

        public FlappingGameManager flappingGameManager;
        public TMP_Text scoreText;

        [Header("Phone 관련")]
        public PhoneControl phoneControl;
        public string miniGameName;
        public bool isMiniGameAddToApp;

        public int eventID;
        
        public void Start()
        {
            
            if (questId != -1)
            {
            }

            if (flappingGameManager)
            {
                if (scoreText) flappingGameManager.score.Subscribe(value => scoreText.text = $"{value}");
            }

            SceneUtil.AsyncAddHome(scene =>
            {
                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                {
                    var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                    if (app != null)
                    {
                        phoneControl.applicationControl.AddApp(app);
                        phoneControl.applicationControl.OpenApp(app);
                    }
                }
            });

            if (isMiniGameAddToApp)
            {
                if (miniGameName == "Running Game")
                {
                    SceneUtil.AsyncAddRunningGame(scene =>
                    {
                        foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                        {
                            var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                            if(app != null) phoneControl.applicationControl.AddApp(app);
                        }
                    });
                }
                else
                {
                    SceneUtil.AsyncAddFlappingGame(scene =>
                    {
                        foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                        {
                            var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                            if(app != null) phoneControl.applicationControl.AddApp(app);
                        }
                    });
                    SceneUtil.AsyncAddRunningGame(scene =>
                    {
                        foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                        {
                            var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                            if(app != null) phoneControl.applicationControl.AddApp(app);
                        }
                    });
                }
            }
        }

        public void MakeQuest()
        {
            var setting = QuestSettingProviderHelper.setting;
            var e = setting.InstantiateQuest(eventID);
            e.Play();
        }
    }
    
    #if UNITY_EDITOR

    [CustomEditor(typeof(Seo_Test))]
    public class Seo_TestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as Seo_Test;

            if (GUILayout.Button("퀘스트 생성"))
            {
                script.MakeQuest();
            }
        }
    }
    
    #endif
}
