using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Util;

namespace Manager
{
    public class SoundManager : Singleton<SoundManager>
    {
        public SoundManagerSetting setting;
        public AudioMixer mixer => setting.mixer;
        public Canvas soundCanvas;
        public RectTransform soundCanvasRectTransform;

        private Dictionary<string, AudioSource> audioSourceDictionary = new Dictionary<string, AudioSource>();
 
        public void Awake()
        {
            setting = SoundManagerSettingsProviderHelper.setting;
            Debug.Assert(setting != null, $"Sound Manager Setting 스크립터블 오브젝트가 존재하지 않습니다.");
            if(ReferenceEquals(setting, null)) return;

            if (setting.isInstantiate)
            {
                setting.InstantiateGroupBlock(out soundCanvas);
                soundCanvasRectTransform = soundCanvas.GetComponent<RectTransform>();
            }

            AudioSourcesGenerate();
            var groups = mixer.FindMatchingGroups("");
            foreach (AudioMixerGroup audioMixerGroup in groups)
                SetVolume(audioMixerGroup.name, GetVolume(audioMixerGroup.name));
        }
        
        public void AudioSourcesGenerate()
        {
            // 본래 있던 오디오 소스 삭제
            foreach (AudioSource audioSource in audioSourceDictionary.Values)
                Destroy(audioSource.gameObject);
            audioSourceDictionary.Clear();
            
            // 새로운 오디오 소스를 생성
            // mixer의 있는 그룹 수 만큼 생성
            var groups = mixer.FindMatchingGroups("");
            foreach (AudioMixerGroup audioMixerGroup in groups)
            {
                GameObject obj = new GameObject { name = audioMixerGroup.name };
                var audioSource = obj.AddComponent<AudioSource>();

                audioSource.transform.SetParent(transform);
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                if (audioSource.name == "BGM") audioSource.loop = true;
                
                audioSourceDictionary.TryAdd(audioMixerGroup.name, audioSource);
            }
        }

        public AudioSource GetAudioSource(string groupName)
        {
            audioSourceDictionary.TryGetValue(groupName, out var audioSource);
            return audioSource;
        }

        public AudioSource GetBGMSource() => GetAudioSource("BGM");
        
        public void SetVolume(string volumeName, float value)
        {
            if(ReferenceEquals(setting, null)) return;

            setting.mixer.SetFloat(volumeName,Mathf.Clamp(value - 80f, -80f, 0f));
            PlayerPrefs.SetFloat($"{nameof(SoundManager)}{SoundExtension.Volume}{volumeName}", Mathf.Clamp(value, 0f, 100f));
        }

        public float GetVolume(string volumeName)
        {
            if(ReferenceEquals(setting, null)) return 0f;
            
            if(PlayerPrefs.HasKey($"{nameof(SoundManager)}{SoundExtension.Volume}{volumeName}"))
                return PlayerPrefs.GetFloat($"{nameof(SoundManager)}{SoundExtension.Volume}{volumeName}");
            return setting.mixer.GetFloat(volumeName, out float value) ? value : 0f;
        }
    }
}
