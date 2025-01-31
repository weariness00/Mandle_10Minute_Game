using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Manager
{
    public class SoundManager : Singleton<SoundManager>
    {
        public SoundManagerSetting setting;

        private const string mixerPath = "Data/Manager/Sound/Sound Manager Setting";
        private static readonly string Volume = "Volume"; 

        public void Awake()
        {
            setting = Resources.Load<SoundManagerSetting>(mixerPath);
            Debug.Assert(setting != null, $"Resources/{mixerPath}\n경로에 Sound Manager Setting 스크립터블 오브젝트가 존재하지 않습니다.");
            if(ReferenceEquals(setting, null)) return;
            var audioUIDictionary = setting.InstantiateGroupBlock();
            
            foreach (var group in setting.mixer.FindMatchingGroups(string.Empty))
            {
                if (audioUIDictionary.TryGetValue(group.name, out var block))
                {
                    block.Item2.onValueChanged.AddListener(value => SetVolume(group.name, value * 100f));
                }
                float value = PlayerPrefs.GetFloat($"{nameof(SoundManager)}{Volume}{group.name}");
                SetVolume(group.name, value);
            }
        }
        
        public void SetVolume(string volumeName, float value)
        {
            if(ReferenceEquals(setting, null)) return;

            setting.mixer.SetFloat(volumeName, value - 80f);
            PlayerPrefs.SetFloat($"{nameof(SoundManager)}{Volume}{volumeName}", value - 80f);
        }

        public float GetVolume(string volumeName)
        {
            if(ReferenceEquals(setting, null)) return 0f;

            return setting.mixer.GetFloat(volumeName, out float value) ? value + 80f : 0f;
        }
    }
}
