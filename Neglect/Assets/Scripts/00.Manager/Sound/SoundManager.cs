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

            if (setting.isInstantiate)
            {
                setting.InstantiateGroupBlock(out soundCanvas);
                soundCanvasRectTransform = soundCanvas.GetComponent<RectTransform>();
            }
        }
        
        public void SetVolume(string volumeName, float value)
        {
            if(ReferenceEquals(setting, null)) return;

            setting.mixer.SetFloat(volumeName, value - 80f);
            PlayerPrefs.SetFloat($"{nameof(SoundManager)}{Volume}{volumeName}", value);
        }

        public float GetVolume(string volumeName)
        {
            if(ReferenceEquals(setting, null)) return 0f;
            
            if(PlayerPrefs.HasKey($"{nameof(SoundManager)}{Volume}{volumeName}"))
                return PlayerPrefs.GetFloat($"{nameof(SoundManager)}{Volume}{volumeName}");
            return setting.mixer.GetFloat(volumeName, out float value) ? value : 0f;
        }
    }
}
