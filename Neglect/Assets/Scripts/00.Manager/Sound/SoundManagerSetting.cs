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
        public bool isInstantiate;
        
        [Space]
        public AudioMixer mixer;
        public Canvas audioCanvasPrefab; // 오디오 캔버스
        public SoundBlock groupBlockPrefab; // 오디오 슬라이드 프리펩
        public string groupParentName; // 슬라이드 프리펩 부모 이름
        
        public List<SoundBlock> InstantiateGroupBlock(out Canvas audioCanvas)
        {
            audioCanvas = Instantiate(audioCanvasPrefab);
            Transform groupParent = audioCanvas.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == groupParentName);
            if (groupParent == null)
            {
                Debug.LogError($"{groupParentName}가 존재하지 않습니다.");
                return null;
            }

            List<SoundBlock> blockList = new();
            foreach (var group in mixer.FindMatchingGroups(string.Empty))
            {
                var block = Instantiate(groupBlockPrefab, groupParent);
                block.name = group.name;
                block.slider.value = SoundManager.Instance.GetVolume(group.name);
                block.Initialize(group);
                blockList.Add(block);
            }

            return blockList;
        }
    }
}