using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Manager
{
    [CreateAssetMenu(fileName = "Sound Manager Setting", menuName = "Manager/Sound", order = 0)]
    public class SoundManagerSetting : ScriptableObject
    {
        public AudioMixer mixer;
        public Canvas audioCanvasPrefab;
        public GameObject groupBlockPrefab;

        public string groupParentName;
        
        public Dictionary<string, Tuple<TMP_Text, Slider>> InstantiateGroupBlock()
        {
            var audioCanvas = Instantiate(audioCanvasPrefab);
            DontDestroyOnLoad(audioCanvas);
            
            Transform groupParent = audioCanvas.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == groupParentName);
            if (groupParent == null)
            {
                Debug.LogError($"{groupParentName}가 존재하지 않습니다.");
                return null;
            }

            Dictionary<string, Tuple<TMP_Text, Slider>> blocks = new();
            foreach (var group in mixer.FindMatchingGroups(string.Empty))
            {
                var groupObj = Instantiate(groupBlockPrefab, groupParent);
                groupObj.name = group.name;
                Tuple<TMP_Text, Slider> block = new(groupObj.GetComponentInChildren<TMP_Text>(), groupObj.GetComponentInChildren<Slider>());
                block.Item1.text = group.name;
                
                blocks.Add(group.name, block);
            }

            return blocks;
        }
    }
}