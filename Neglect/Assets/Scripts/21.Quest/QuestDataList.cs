using GamePlay;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util;

namespace Quest
{
    [CreateAssetMenu(fileName = "Quest Data List", menuName = "Game/Quest List", order = 0)]
    public class QuestDataList : ScriptableObject
    {
        [SerializeField] private QuestBase[] questPrefabList;
        [SerializeField] private TextAsset csvFile;
        // [SerializeField] private List<QuestScriptableObject> easyQuestList;
        // [SerializeField] private List<QuestScriptableObject> normalQuestList;
        // [SerializeField] private List<QuestScriptableObject> hardQuestList;
        // [SerializeField] private List<QuestScriptableObject> hiddenQuestList;

        public QuestBase GetQuestID(int id)
        {
            return questPrefabList.First(q => q.questID == id);
        }

        public QuestBase GetQuestName(string questName)
        {
            return questPrefabList.First(q => q.questName == questName);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 모든 퀘스트 list에 저장
        /// </summary>
        public void SetQuestList()
        {
            questPrefabList = Resources.LoadAll<QuestBase>("Quest");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Quest 데이터 저장 완료");
        }
        
        [MenuItem("Assets/Create/Game/Init CSV")]
        public static void InitCSV()
        {
            QuestSettingProviderHelper.setting.SetCSVData();
        }
        
        public void SetCSVData()
        {
            Debug.Assert(csvFile != null, "CSV파일이 없어서 퀘스트 데이터를 셋팅하지 못했습니다.");
            var path = AssetDatabase.GetAssetPath(csvFile);
            path = path.Replace("Assets/", "");
            path = path.Replace("Resources/", "");
            path = path.Replace(".asset", "");
            var csv = CSVReader.Read(path);
            foreach (Dictionary<string,object> data in csv)
            {
                int id = (int)data["이벤트 ID"];
                string eName = (string)data["이벤트 이름"];
                QuestLevel level = (QuestLevel)data["이벤트 난이도"];
                int nextID = (int)data["후속 이벤트 ID"];

                var quest = questPrefabList.First(q => q.questID == id);
                if (quest != null)
                {
                    quest.questName = eName;
                    quest.level = level;
                    quest.nextQuestID = nextID;
                    
                    EditorUtility.SetDirty(quest);
                    PrefabUtility.SavePrefabAsset(quest.gameObject);
                }
            }
            AssetDatabase.Refresh();
        }
#endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(QuestDataList))]
    class QuestDataListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = target as QuestDataList;
            if (GUILayout.Button("Init Quest List"))
            {
                script.SetQuestList();
            }
            if (GUILayout.Button("Init CSV"))
                script.SetCSVData();
        }
    }
#endif
}