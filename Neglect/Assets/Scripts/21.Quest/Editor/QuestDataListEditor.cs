using UnityEditor;
using UnityEngine;

namespace Quest.Editor
{
    [CustomEditor(typeof(QuestDataList))]
    class QuestDataListEditor : UnityEditor.Editor
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
                script.InitData();
        }
    }
}