using GamePlay;
using GamePlay.MiniGame.FlappingGame;
using GamePlay.Phone;
using Manager;
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
        [Header("Phone 관련")]
        public PhoneControl phoneControl;
        public string miniGameName;
        public bool isMiniGameAddToApp;

        public int eventID;
        
        public void Start()
        {
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
            }
        }

        public void MakeQuest()
        {
            var setting = QuestSettingProviderHelper.setting;
            var e = setting.InstantiateEvent(eventID);
            QuestManager.Instance.AddQuestQueue(e);
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

            if (GUILayout.Button("퀘스트 결과창 생성"))
            {
                GameManager.Instance.GameClear();
            }
            
        }
    }
    
    #endif
}
