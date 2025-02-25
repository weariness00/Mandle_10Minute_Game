using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Narration
{
#if UNITY_EDITOR
    public class NarrationSettingProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(
                "Project/Game Play/Narration", 
                SettingsScope.Project, 
                new []{ "Scriptable", "Game Play", "Narration", "List" })
            {
                guiHandler = (searchContext) =>
                {
                    // 설정 창에 표시할 UI
                    NarrationSettingProviderHelper.IsDebug = EditorGUILayout.Toggle("Is Debug", NarrationSettingProviderHelper.IsDebug); 
                    EditorGUILayout.LabelField("Talking Data", EditorStyles.boldLabel);
                    var setting = NarrationSettingProviderHelper.setting = (NarrationScriptableObject)EditorGUILayout.ObjectField(
                        $"Talking Data",
                        NarrationSettingProviderHelper.setting,
                        typeof(NarrationScriptableObject),
                        false
                    );

                    if (setting != null)
                    {
                        Editor.CreateEditor(setting).OnInspectorGUI();
                    }
                    
                    // setting이 변경되었을 경우 Save() 호출
                    if (GUI.changed)
                    {
                        NarrationSettingProviderHelper.Save();
                    }
                },
            };
        
            return provider;
        }
    }
    
#endif
    
    [Serializable]
    public struct NarrationSettingJson
    {
        public string SettingPath;
    }

    public static class NarrationSettingProviderHelper
    {
        private static bool _IsDebug = true;
        public static NarrationScriptableObject setting;

        private static readonly string JsonDirectory = "Assets/Resources/Data/Json";
        private static readonly string SettingJsonPath = "Resources/Data/Json/Narration Data.json";
        private static readonly string DefaultKey = "Game,Narration";
        private static readonly string DebugKey = "IsDebug";
        private static readonly string SettingKey = nameof(NarrationScriptableObject);

#if UNITY_EDITOR
        public static bool IsDebug
        {
            get => _IsDebug;
            set
            {
                if (_IsDebug != value)
                    EditorPrefs.SetBool(DefaultKey + DebugKey, value);
                _IsDebug = value;
            }
        }

        static NarrationSettingProviderHelper()
        {
            IsDebug = EditorPrefs.GetBool(DefaultKey + DebugKey);
            if (!Directory.Exists(JsonDirectory))
                Directory.CreateDirectory(JsonDirectory);
            AssetDatabase.Refresh();
            Load();
        }

        public static void Save()
        {
            if (setting != null)
            {
                string path = AssetDatabase.GetAssetPath(setting);
                NarrationSettingJson data = new();
                data.SettingPath = path;
                string json = JsonUtility.ToJson(data, true);

                File.WriteAllText(Path.Combine(Application.dataPath, SettingJsonPath), json);
                AssetDatabase.Refresh();

                EditorPrefs.SetString(DefaultKey + SettingKey, path);
            }
        }

        public static void Load()
        {
            if (EditorPrefs.HasKey(DefaultKey + SettingKey))
            {
                string settingPath = EditorPrefs.GetString(DefaultKey + SettingKey, string.Empty);
                setting = AssetDatabase.LoadAssetAtPath<NarrationScriptableObject>(settingPath);
                Debug.Assert(setting != null, $"해당 경로에 {nameof(NarrationScriptableObject)} 데이터가 존재하지 않습니다.");
            }
        }
#else
        static NarrationSettingProviderHelper()
        {
            Load();
        }

        public static void Load()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<NarrationSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                setting = Resources.Load<NarrationScriptableObject>(path);
            }
        }
#endif
    }
}