using Quest;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamePlay
{
    public class QuestSettingProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(
                "Project/Game Play/Quest", 
                SettingsScope.Project, 
                new []{ "Scriptable", "Game Play", "Quest", "List" })
            {
                guiHandler = (searchContext) =>
                {
                    // 설정 창에 표시할 UI
                    QuestSettingProviderHelper.IsDebug = EditorGUILayout.Toggle("Is Debug", QuestSettingProviderHelper.IsDebug); 
                    EditorGUILayout.LabelField("Quest Data", EditorStyles.boldLabel);
                    QuestSettingProviderHelper.setting = (QuestDataList)EditorGUILayout.ObjectField(
                        $"Quest Data List",
                        QuestSettingProviderHelper.setting,
                        typeof(QuestDataList),
                        false
                    );
                    
                    // setting이 변경되었을 경우 Save() 호출
                    if (GUI.changed)
                    {
                        QuestSettingProviderHelper.Save();
                    }
                },
            };
        
            return provider;
        }
    }
    
    [Serializable]
    public struct QuestSettingJson
    {
        public string SettingPath;
    }

    public static class QuestSettingProviderHelper
    {
        private static bool _IsDebug = true;
        public static QuestDataList setting;

        private static readonly string JsonDirectory = "Assets/Resources/Data/Json";
        private static readonly string SettingJsonPath = "Resources/Data/Json/Quest Data List.json";
        private static readonly string DefaultKey = "Game,Quest List";
        private static readonly string DebugKey = "IsDebug";
        private static readonly string SettingKey = nameof(QuestDataList);

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

        static QuestSettingProviderHelper()
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
                QuestSettingJson data = new();
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
                setting = AssetDatabase.LoadAssetAtPath<QuestDataList>(settingPath);
                Debug.Assert(setting != null, "해당 경로에 Sound Manager Setting 데이터가 존재하지 않습니다.");
            }
        }
#else
        static QuestSettingProviderHelper()
        {
            Load();
        }

        public static void Load()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<QuestSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                setting = Resources.Load<QuestDataList>(path);
            }
        }
#endif
    }
}