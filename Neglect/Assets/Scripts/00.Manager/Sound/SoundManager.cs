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

        private static readonly string Volume = "Volume"; 
 
        public void Awake()
        {
            setting = SoundManagerSettingsProviderHelper.setting;
            Debug.Assert(setting != null, $"Sound Manager Setting 스크립터블 오브젝트가 존재하지 않습니다.");
            if(ReferenceEquals(setting, null)) return;

            setting.InstantiateGroupBlock(out soundCanvas);
            soundCanvasRectTransform = soundCanvas.GetComponent<RectTransform>();
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
