using GamePlay;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Quest
{
    [CreateAssetMenu(fileName = "Quest Data List", menuName = "Game/Quest List", order = 0)]
    public class QuestDataList : ScriptableObject
    {
        [SerializeField][Tooltip("퀘스트 로직이 담긴 프리펩")] private QuestBase[] questList;
        [SerializeField][Tooltip("퀘스트가 소환할 프리펩")] private QuestPrefab[] prefabList;
        [SerializeField] private TextAsset questDataTableCSV;
        [SerializeField] private TextAsset prefabDataTableCSV;
        // [SerializeField] private List<QuestScriptableObject> easyQuestList;
        // [SerializeField] private List<QuestScriptableObject> normalQuestList;
        // [SerializeField] private List<QuestScriptableObject> hardQuestList;
        // [SerializeField] private List<QuestScriptableObject> hiddenQuestList;

        public QuestBase GetQuestID(int id)
        {
            return questList.FirstOrDefault(q => q.questID == id);
        }

        public QuestBase GetQuestName(string questName)
        {
            return questList.FirstOrDefault(q => q.questName == questName);
        }

        public QuestPrefab GetPrefabID(int id)
        {
            return prefabList.FirstOrDefault(q => q.id == id);
        }

        public QuestPrefab GetPrefabName(string Name)
        {
            return prefabList.FirstOrDefault(q => q.name == Name);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 모든 퀘스트 list에 저장
        /// </summary>
        public void SetQuestList()
        {
            questList = Resources.LoadAll<QuestBase>("Quest");
            prefabList = Resources.LoadAll<QuestPrefab>("Quest");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Quest 데이터 저장 완료");
        }
        
        [MenuItem("Assets/Create/Game/Init CSV")]
        public static void InitData()
        {
            QuestSettingProviderHelper.setting.SetQuestCSVData();
            QuestSettingProviderHelper.setting.SetPrefabCSVData();
        }
        
        public void SetQuestCSVData()
        {
            var questCSV = questDataTableCSV.GetCSV();
            foreach (Dictionary<string,object> data in questCSV)
            {
                var id = data.DynamicCast<int>("EventID");
                var eName = data.DynamicCast<string>("Name");
                var level = data.DynamicCast<QuestLevel>("Level");
                var nextQuestID = data.DynamicCast<int>("NextEventID");
                var spawnPrefabNameArray = data.DynamicCast<string[]>("PrefabName");

                var quest = GetQuestID(id);
                if (quest != null)
                {
                    quest.questName = eName;
                    quest.level = level;
                    quest.nextQuest = GetQuestID(nextQuestID);
                    quest.questPrefabList.Clear();
                    foreach (string prefabName in spawnPrefabNameArray)
                    {
                        var prefab = GetPrefabName(prefabName);
                        if(prefab != null) quest.questPrefabList.Add(prefab);
                    }
                    EditorUtility.SetDirty(quest);
                    PrefabUtility.SavePrefabAsset(quest.gameObject);
                }
            }
            AssetDatabase.Refresh();
        }

        public void SetPrefabCSVData()
        {
            var prefabCSV = prefabDataTableCSV.GetCSV();
            foreach (Dictionary<string, object> data in prefabCSV)
            {
                var id = data.DynamicCast<int>("PrefabID");
                var prefabName = data.DynamicCast<string>("PrefabName");

                var perfab = GetPrefabID(id);
                perfab.name = prefabName;
                EditorUtility.SetDirty(perfab);
                PrefabUtility.SavePrefabAsset(perfab.gameObject);
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
                script.SetQuestCSVData();
        }
    }
#endif
}