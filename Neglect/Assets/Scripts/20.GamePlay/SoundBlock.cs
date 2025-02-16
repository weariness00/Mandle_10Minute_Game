using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GamePlay
{
    public class SoundBlock : Manager.SoundBlock
    {
        public Image icon;

        public override void Initialize(AudioMixerGroup group)
        {
            base.Initialize(group);
        }
    }
}

