using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Manager
{
    public class SoundBlock : MonoBehaviour
    {
        public Slider slider;

        public virtual void Initialize(AudioMixerGroup group)
        {
            slider.onValueChanged.AddListener(value => SoundManager.Instance.SetVolume(group.name, value * 100f));
            float value = PlayerPrefs.GetFloat($"{nameof(SoundManager)}{SoundExtension.Volume}{group.name}");
            SoundManager.Instance.SetVolume(group.name, value);
        }
    }
}

