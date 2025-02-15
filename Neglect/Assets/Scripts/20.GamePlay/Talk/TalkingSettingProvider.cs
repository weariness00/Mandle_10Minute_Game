using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Talk
{
#if UNITY_EDITOR
    public class TalkingSettingProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(
                "Project/Game Play/Talking", 
                SettingsScope.Project, 
                new []{ "Scriptable", "Game Play", "Talking", "List" })
            {
                guiHandler = (searchContext) =>
                {
                    // 설정 창에 표시할 UI
                    TalkingSettingProviderHelper.IsDebug = EditorGUILayout.Toggle("Is Debug", TalkingSettingProviderHelper.IsDebug); 
                    EditorGUILayout.LabelField("Talking Data", EditorStyles.boldLabel);
                    var setting = TalkingSettingProviderHelper.setting = (TalkingScriptableObject)EditorGUILayout.ObjectField(
                        $"Talking Data",
                        TalkingSettingProviderHelper.setting,
                        typeof(TalkingScriptableObject),
                        false
                    );

                    if (setting != null)
                    {
                        Editor.CreateEditor(setting).OnInspectorGUI();
                    }
                    
                    // setting이 변경되었을 경우 Save() 호출
                    if (GUI.changed)
                    {
                        TalkingSettingProviderHelper.Save();
                    }
                },
            };
        
            return provider;
        }
    }
    
#endif
    
    [Serializable]
    public struct TalkingSettingJson
    {
        public string SettingPath;
    }

    public static class TalkingSettingProviderHelper
    {
        private static bool _IsDebug = true;
        public static TalkingScriptableObject setting;

        private static readonly string JsonDirectory = "Assets/Resources/Data/Json";
        private static readonly string SettingJsonPath = "Resources/Data/Json/Talking Data.json";
        private static readonly string DefaultKey = "Game,Talking";
        private static readonly string DebugKey = "IsDebug";
        private static readonly string SettingKey = nameof(TalkingScriptableObject);

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

        static TalkingSettingProviderHelper()
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
                TalkingSettingJson data = new();
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
                setting = AssetDatabase.LoadAssetAtPath<TalkingScriptableObject>(settingPath);
                Debug.Assert(setting != null, $"해당 경로에 {nameof(TalkingScriptableObject)} 데이터가 존재하지 않습니다.");
            }
        }
#else
        static TalkingSettingProviderHelper()
        {
            Load();
        }

        public static void Load()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<TalkingSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                setting = Resources.Load<TalkingScriptableObject>(path);
            }
        }
#endif
    }
}