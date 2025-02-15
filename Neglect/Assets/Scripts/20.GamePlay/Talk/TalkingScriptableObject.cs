using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util;

namespace GamePlay.Talk
{
    [CreateAssetMenu(fileName = "Talking Data", menuName = "Game/Talking", order = 0)]
    public class TalkingScriptableObject : ScriptableObject
    {
        public static TalkingScriptableObject Instance => TalkingSettingProviderHelper.setting;
        
        [SerializeField] private TalkingData[] talkingDataArray;
        [SerializeField] private TextAsset talkDataCSV;
        [SerializeField] private TextAsset textDataCSV;

        public TalkingData GetTalkData(int id)
        {
            var index = Array.BinarySearch(talkingDataArray, id);
            return index >= 0 ? talkingDataArray[index] : null;
        }

#if UNITY_EDITOR
        public void InitCSV()
        {
            Dictionary<int, string> textDataDictionary = new();
            { // Text Data Table 초기화
                var csv = textDataCSV.ReadHorizon();
                foreach (Dictionary<string, object> data in csv)
                {
                    var id = data.DynamicCast<int>("TextID");
                    var text = data.DynamicCast<string>("TextContent");
                    if(!textDataDictionary.TryAdd(id, text))
                        Debug.LogWarning($"{id}에 이미 문자열이 할당되어 있습니다.");
                }
            }
            { // Talking Data Table 초기화
                var csv = talkDataCSV.ReadHorizon();
                talkingDataArray = new TalkingData[csv.Count];

                for (var i = 0; i < csv.Count; i++)
                {
                    var data = csv[i];
                    var talkID = data.DynamicCast<int>("EventID");
                    TalkingData talk = new();
                    talk.id = talkID;
                    talkingDataArray[i] = talk;
                }
                Array.Sort(talkingDataArray, (a,b) => a.id.CompareTo(b.id));
                foreach (var data in csv)
                {
                    var talkID = data.DynamicCast<int>("EventID");
                    var mainTextID = data.DynamicCast<int>("MainTextID");
                    var positiveTextIDArray = data.DynamicCast<int[]>("PositiveTextList");
                    var negativeTextIDArray = data.DynamicCast<int[]>("NegativeTextList");
                    var positiveTalkID = data.DynamicCast<int>("PositiveTalkID");
                    var negativeTalkID = data.DynamicCast<int>("NegativeTalkID");

                    var talk = GetTalkData(talkID);
                    talk.mainText = textDataDictionary[mainTextID];
                    talk.positiveTextArray = positiveTextIDArray.Select(id => textDataDictionary[id]).ToArray();
                    talk.negativeTextArray = negativeTextIDArray.Select(id => textDataDictionary[id]).ToArray();
                    talk.positiveResultTalk = GetTalkData(positiveTalkID);
                    talk.negativeResultTalk = GetTalkData(negativeTalkID);
                }
            }
        }
#endif
    }

    [CustomEditor(typeof(TalkingScriptableObject))]
    public class TalkingScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as TalkingScriptableObject;
            if (GUILayout.Button("CSV 적용"))
            {
                script.InitCSV();
            }
        }
    }
}