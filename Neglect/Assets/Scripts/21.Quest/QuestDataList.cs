using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Quest
{
    [CreateAssetMenu(fileName = "Quest Data List", menuName = "Game/Quest List", order = 0)]
    public class QuestDataList : ScriptableObject
    {
        public static QuestDataList Instance => QuestSettingProviderHelper.setting;
        
        [SerializeField] [Tooltip("퀘스트 데이터 테이블")] private EventData[] eventDataArray;
        [SerializeField] [Tooltip("퀘스트 로직이 담긴 프리펩")] private QuestBase[] questArray;

        public QuestBase InstantiateEvent(int id)
        {
            var data = GetEventID(id);
            if (data == null) return null;
            var quest = Instantiate(data.prefab);
            quest.eventData = data;
            return quest;
        }

        public QuestBase InstantiateRandomEvent()
        {
            UniqueRandom eventRandom = new(0, eventDataArray.Length);
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
        }

        public void SetEventCSV()
        {
            var eventCSV = eventDataTableCSV.ReadHorizon();
            eventDataArray = new EventData[eventCSV.Count];
            SetQuestTextData(out var questTextArray);
            for (var i = 0; i < eventCSV.Count; i++)
            {
                var data = new EventData();
                var csv = eventCSV[i];
                data.id = csv.DynamicCast<int>("EventID", -1);
                eventDataArray[i] = data;
            }
            
            Array.Sort(eventDataArray, (a,b) => a.id.CompareTo(b.id));

            foreach (var csv in eventCSV)
            {
                var id = csv.DynamicCast<int>("EventID");
                var textList = csv.DynamicCast<int[]>("TextListID", Array.Empty<int>());
                var data = GetEventID(id);
                data.level = csv.DynamicCast<QuestLevel>("Level", QuestLevel.None);
                data.prefab = GetQuestID(csv.DynamicCast<int>("PrefabID", -1));
                data.textArray = questTextArray.Where(d => textList.FirstOrDefault(ti => ti == d.id) != 0).Select(d => d.text).ToArray();

                data.acceptEvent = GetEventID(csv.DynamicCast<int>("AcceptEventID", -1));
                data.ignoreEvent = GetEventID(csv.DynamicCast<int>("IgnoreEventID", -1));
                
                Debug.Assert(data.prefab != null, "Event Data에 프리펩이 존재하지 않습니다.");
            }

            //후속 이벤트들 할당
            for (var i = 0; i < eventCSV.Count; i++)
            {
                var csv = eventCSV[i];
                eventDataArray[i].acceptEvent = GetEventID(csv.DynamicCast<int>("AcceptEventID", -1));
                eventDataArray[i].ignoreEvent = GetEventID(csv.DynamicCast<int>("IgnoreEventID", -1));
            }
        }

        public void SetQuestTextData(out QuestTextData[] questTextArray)
        {
            var questTextCSV = textDataTableCSV.ReadHorizon();
            questTextArray = new QuestTextData[questTextCSV.Count];

            for (var i = 0; i < questTextCSV.Count; i++)
            {
                var csv = questTextCSV[i];
                questTextArray[i] = new() 
                    { 
                        id = csv.DynamicCast<int>("ID"),
                        text = csv.DynamicCast<string>("Content") 
                    };
            }
        }
#endif
    }
}