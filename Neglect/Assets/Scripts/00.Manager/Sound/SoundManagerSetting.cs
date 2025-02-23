using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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
    
#if UNITY_EDITOR

    [CustomEditor(typeof(SoundManagerSetting))]
    public class SoundManagerSettingEditor : Editor
    {
        private float volume = 100;
        public override void OnInspectorGUI()
        {
            var script = target as SoundManagerSetting;
            
            volume = EditorGUILayout.FloatField("재설정 Volume 값", volume);
            if (GUILayout.Button("모든 사운드 재설정"))
            {
                foreach (var group in script.mixer.FindMatchingGroups(string.Empty))
                {
                    SoundExtension.SetVolume(group.name, volume);
                }
            }
            
            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
#endif
}