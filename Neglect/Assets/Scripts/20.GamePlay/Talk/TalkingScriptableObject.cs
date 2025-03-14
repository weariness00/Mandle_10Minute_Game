﻿using System;
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
                var csv = textDataCSV.Read();
                foreach (Dictionary<string, object> data in csv)
                {
                    var id = data.DynamicCast("TextID", -1);
                    var text = data.DynamicCast("TextContent", "");
                    if(!textDataDictionary.TryAdd(id, text))
                        Debug.LogWarning($"{id}에 이미 문자열이 할당되어 있습니다.");
                }
            }
            { // Talking Data Table 초기화
                var csv = talkDataCSV.Read();
                List<TalkingData> talkList = new();

                for (var i = 0; i < csv.Count; i++)
                {
                    var data = csv[i];
                    var talkID = data.DynamicCast<int>("TalkingID", -1);
                    if(talkID == -1) continue;
                    TalkingData talk = new();
                    talk.id = talkID;
                    talkList.Add(talk);
                }
                talkingDataArray = talkList.ToArray();
                Array.Sort(talkingDataArray, (a,b) => a.id.CompareTo(b.id));
                foreach (var data in csv)
                {
                    var talkID = data.DynamicCast("TalkingID", -1);
                    if(talkID == -1) continue;
                    
                    var mainTextID = data.DynamicCast("MainTextID", -1);
                    var positiveTextIDArray = data.DynamicCast("PositiveTextList", Array.Empty<int>());
                    var negativeTextIDArray = data.DynamicCast("NegativeTextList", Array.Empty<int>());
                    var positiveTalkID = data.DynamicCast("PositiveTalkID", -1);
                    var negativeTalkID = data.DynamicCast("NegativeTalkID", -1);
                    var positiveScore = data.DynamicCast("PositiveValue", 0);
                    var negativeScore = data.DynamicCast("NegativeValue", 0);

                    var talk = GetTalkData(talkID);
                    talk.mainText = textDataDictionary[mainTextID];
                    talk.positiveTextArray = positiveTextIDArray.Select(id => textDataDictionary[id]).ToArray();
                    talk.negativeTextArray = negativeTextIDArray.Select(id => textDataDictionary[id]).ToArray();
                    talk.positiveResultTalkID = positiveTalkID;
                    talk.negativeResultTalkID = negativeTalkID;
                    talk.positiveScore = positiveScore;
                    talk.negativeScore = negativeScore;
                }
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }

#if UNITY_EDITOR
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
#endif
}