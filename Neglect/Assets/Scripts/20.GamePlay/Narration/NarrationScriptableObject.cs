using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Util;

namespace GamePlay.Narration
{
    [CreateAssetMenu(fileName = "Narration Data", menuName = "Game/Narration", order = 0)]
    public class NarrationScriptableObject : ScriptableObject
    {
        public static NarrationScriptableObject Instance => NarrationSettingProviderHelper.setting;

        [SerializeField] private NarrationData[] narrationArray;
        
        public NarrationData GetNarrationID(int id)
        {
            var index = Array.BinarySearch(narrationArray, id);
            return index >= 0 ? narrationArray[index] : null;
        }
        
#if UNITY_EDITOR
        [SerializeField] private TextAsset narrationCSV;
        [SerializeField] private TextAsset textCSV;

        public void InitCSV()
        {
            Dictionary<int, string> textDataDictionary = new();
            { // Text Data Table 초기화
                var csv = textCSV.ReadHorizon();
                foreach (Dictionary<string, object> data in csv)
                {
                    var id = data.DynamicCast<int>("TextID");
                    var text = data.DynamicCast<string>("TextContent");
                    if(!textDataDictionary.TryAdd(id, text))
                        Debug.LogWarning($"{id}에 이미 문자열이 할당되어 있습니다.");
                }
            }
            { // Narration Data Table 초기화
                var csv = narrationCSV.ReadHorizon();
                List<NarrationData> list = new();
                foreach (Dictionary<string, object> data in csv)
                {
                    var id = data.DynamicCast("NarrationID", -1);
                    if (id == -1) continue;

                    NarrationData narration = new();
                    var narrationName = data.DynamicCast("NarrationName", "");
                    var narrationTextID = data.DynamicCast("NarrationText", -1);
                    var stayDuration = data.DynamicCast("NarrationTime", 2);

                    narration.id = id;
                    narration.name = narrationName;
                    narration.text = textDataDictionary.GetValueOrDefault(narrationTextID, "");
                    narration.stayDuration = stayDuration;
                    
                    list.Add(narration);
                }

                narrationArray = list.ToArray();
                Array.Sort(narrationArray, (a,b) => a.id.CompareTo(b.id));
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
#endif
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(NarrationScriptableObject))]
    public class NarrationScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as NarrationScriptableObject;
            if (GUILayout.Button("CSV 초기화"))
            {
                script.InitCSV();
            }
            
            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
#endif
}