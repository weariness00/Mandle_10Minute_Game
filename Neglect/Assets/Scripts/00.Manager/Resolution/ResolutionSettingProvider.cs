using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manager
{
#if UNITY_EDITOR
    public static class ResolutionSettingUIProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(
                "Project/Managers/Resolution", 
                SettingsScope.Project, 
                new []{ "Scriptable", "Settings", "Manager", "Resolution" })
            {
                guiHandler = (searchContext) =>
                {
                    // 설정 창에 표시할 UI
                    ResolutionSettingProviderHelper.IsDebug = EditorGUILayout.Toggle("Is Debug", ResolutionSettingProviderHelper.IsDebug); 
                    EditorGUILayout.LabelField("Resolution Manager Data", EditorStyles.boldLabel);
                    var setting = ResolutionSettingProviderHelper.setting = (ResolutionSetting)EditorGUILayout.ObjectField(
                        $"Setting Data",
                        ResolutionSettingProviderHelper.setting,
                        typeof(ResolutionSetting),
                        false
                    );

                    if (setting != null)
                    {
                        Editor.CreateEditor(setting).OnInspectorGUI();
                    }
                    
                    // setting이 변경되었을 경우 Save() 호출
                    if (GUI.changed)
                    {
                        ResolutionSettingProviderHelper.Save();
                    }
                },
            };
        
            return provider;
        }
    }
#endif

    [Serializable]
    public struct ResolutionSettingJson
    {
        public string SettingPath;
    }

    public static class ResolutionSettingProviderHelper
    {
        private static bool _IsDebug = true;
        public static ResolutionSetting setting;

        private static readonly string JsonDirectory = "Assets/Resources/Data/Json";
        private static readonly string SettingJsonPath = "Resources/Data/Json/Resolution Setting.json";
        private static readonly string DefaultKey = "Managers,Resolution";
        private static readonly string DebugKey = "IsDebug";
        private static readonly string SettingKey = nameof(ResolutionSetting);

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

        static ResolutionSettingProviderHelper()
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
                ResolutionSettingJson data = new();
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
                setting = AssetDatabase.LoadAssetAtPath<ResolutionSetting>(settingPath);
                Debug.Assert(setting != null, "해당 경로에 Sound Manager Setting 데이터가 존재하지 않습니다.");
            }
            else
            {
                var path = GetDataPath();
                if (path != string.Empty)
                {
                    setting = Resources.Load<ResolutionSetting>(path);
                }
            }
        }
#else
        static ResolutionSettingProviderHelper()
        {
            Load();
        }

        public static void Load()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<ResolutionSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                setting = Resources.Load<ResolutionSetting>(path);
            }
        }
#endif
        public static string GetDataPath()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<SoundManagerSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                return path;
            }

            Debug.LogError($"{SettingJsonPath}에 {nameof(setting)}이 존재 하지 않습니다.");
            return "";
        }
    }
}