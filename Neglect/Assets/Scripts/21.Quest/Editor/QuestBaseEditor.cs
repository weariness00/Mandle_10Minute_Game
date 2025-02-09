using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quest.Editor
{
    [CustomEditor(typeof(QuestBase), true)]
    public class QuestBaseEditor : UnityEditor.Editor
    {
        private static QuestBase prefab;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var script = target as QuestBase;
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(script);
            // 프리팹 인스턴스일 경우 원본을 가져옵니다
            if (prefabType == PrefabAssetType.NotAPrefab)
                script = PrefabUtility.GetCorrespondingObjectFromSource(script) as QuestBase;
            if (script == null)
                script = prefab;

            prefab = script;
            
            var setting = QuestSettingProviderHelper.setting;
            if (setting != null)
            {
                var quest = setting.GetQuestID(script.questID);
                if (quest == null && 
                    GUILayout.Button("Quest List에 추가"))
                {
                    setting.AddQuest(script);
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                if(quest != null &&
                   GUILayout.Button("Quest List에서 제거"))
                {
                    setting.RemoveQuest(script);
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("Quest Data Table로 이동"))
                {
                    Selection.activeObject = setting; // 선택
                    EditorGUIUtility.PingObject(setting); // 강조
                }
            }
        }
    }
}