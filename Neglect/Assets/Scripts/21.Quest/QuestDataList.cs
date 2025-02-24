using System;
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
        public static QuestDataList Instance => QuestSettingProviderHelper.setting;
        
        [SerializeField] [Tooltip("모든 이벤트")] private EventData[] eventDataArray;
        [SerializeField] [Tooltip("메인 이벤트")] private EventData[] mainEventDataArray;
        [SerializeField] [Tooltip("퀘스트 로직이 담긴 프리펩")] private QuestBase[] questArray;
        
        public MinMaxValue<float> questSpawnTimer = new(0, 0, 30,true,true);

        public QuestBase InstantiateEvent(int id)
        {
            var data = GetEventID(id);
            if (data == null) return null;
            var quest = Instantiate(data.prefab);
            quest.eventData = data;
            return quest;
        }
        
        public QuestBase InstantiateMainEvent(int id)
        {
            var data = GetMainEventID(id);
            if (data == null) return null;
            var quest = Instantiate(data.prefab);
            quest.eventData = data;
            return quest;
        }

        public QuestBase InstantiateRandomEvent()
        {
            UniqueRandom eventRandom = new(0, eventDataArray.Length - 1);
            int index = 0;
            EventData data = eventDataArray[index];
            while (!eventRandom.IsEmptyInt)
            {
                index = eventRandom.RandomInt();
                data = eventDataArray[index];
                if(data.id != -1 && data.prefab != null) 
                    break;
            }
            var quest = Instantiate(data.prefab);
            quest.eventData = data;
            return quest;
        }

        public EventData[] GetAllEvent() => eventDataArray;
        public EventData[] GetAllMainEvent() => mainEventDataArray;
        
        public QuestBase GetQuestID(int id)
        {
            var index = Array.BinarySearch(questArray, id);
            return index >= 0 ? questArray[index] : null;
        }

        public EventData GetEventID(int id)
        {
            var index = Array.BinarySearch(eventDataArray, id);
            return index >= 0 ? eventDataArray[index] : null;
        }

        public EventData GetMainEventID(int id)
        {
            var index = Array.BinarySearch(mainEventDataArray, id);
            return index >= 0 ? mainEventDataArray[index] : null;
        }

#if UNITY_EDITOR
        [SerializeField] private TextAsset eventDataTableCSV;
        [SerializeField] private TextAsset textDataTableCSV;
        
        public void AddQuest(QuestBase questPrefab) => questArray = questArray.Concat(new [] {questPrefab}).ToArray();
        public void RemoveQuest(QuestBase questPrefab) => questArray = questArray.Where(q => q.questID != questPrefab.questID).ToArray();
        
        /// <summary>
        /// 모든 퀘스트 list에 저장
        /// </summary>
        public void SetQuestList()
        {
            questArray = Resources.LoadAll<QuestBase>("Quest").ToArray();
            Array.Sort(questArray);
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Quest 데이터 저장 완료");
        }
        
        public void InitData()
        {
            Array.Sort(questArray);
            QuestSettingProviderHelper.setting.SetEventCSV();
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void SetEventCSV()
        {
            var eventCSV = eventDataTableCSV.ReadHorizon();
            SetQuestTextData(out var textDataDictionary);

            List<EventData> eventList = new List<EventData>();
            List<EventData> mainEventList = new List<EventData>();
            for (var i = 0; i < eventCSV.Count; i++)
            {
                var csv = eventCSV[i];
                var data = new EventData();
                var id = csv.DynamicCast<int>("EventID", -1);
                var isMainEvent = csv.DynamicCast<int>("IsMainEvent", 0) == 1;

                if(id == -1) continue;
                
                data.id = id;
                data.isMainEvent = isMainEvent;
                if(isMainEvent) mainEventList.Add(data);
                
                eventList.Add(data);
            }

            eventDataArray = eventList.ToArray();
            mainEventDataArray = mainEventList.ToArray();
            
            Array.Sort(eventDataArray, (a,b) => a.id.CompareTo(b.id));
            Array.Sort(mainEventDataArray, (a,b) => a.id.CompareTo(b.id));
            
            foreach (var csv in eventCSV)
            {
                var id = csv.DynamicCast<int>("EventID", -1);
                if(id == -1) continue;
                var eventName = csv.DynamicCast("Name", "");
                var textList = csv.DynamicCast<int[]>("TextListID", Array.Empty<int>());
                var prefabID = csv.DynamicCast<int>("PrefabID", -1);
                var data = GetEventID(id);
                data.name = eventName;
                data.level = LevelToInt(csv.DynamicCast("Level", ""));
                data.prefab = GetQuestID(prefabID);

                data.acceptEventID = csv.DynamicCast<int>("AcceptEventID", -1);
                data.ignoreEventID = csv.DynamicCast<int>("IgnoreEventID", -1);

                data.textArray = textDataDictionary.Where(d => textList.FirstOrDefault(tid => tid == d.Key) != 0).Select(d => d.Value).ToArray();
    
                data.extraDataIDArray = csv.DynamicCast("ExtraDataID", Array.Empty<int>());
                
                Debug.Assert(data.prefab != null, $"Event Data에 {prefabID}프리펩이 존재하지 않습니다.");
            }

            QuestLevel LevelToInt(string level)
            {
                if (level == "쉬움")
                    return QuestLevel.Easy;
                if (level == "중간")
                    return QuestLevel.Normal;
                if (level == "어려움")
                    return QuestLevel.Hard;
                return QuestLevel.None;
            }
        }

        public void SetQuestTextData(out Dictionary<int, string> textDataDictionary)
        {
            textDataDictionary = new();
            { // Text Data Table 초기화
                var csv = textDataTableCSV.ReadHorizon();
                foreach (Dictionary<string, object> data in csv)
                {
                    var id = data.DynamicCast<int>("TextID");
                    var text = data.DynamicCast<string>("TextContent");
                    if(!textDataDictionary.TryAdd(id, text))
                        Debug.LogWarning($"{id}에 이미 문자열이 할당되어 있습니다.");
                }
            }
        }
#endif
    }
}