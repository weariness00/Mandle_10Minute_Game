using Manager;
using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

namespace GamePlay.Phone
{
    public class PhoneSoundButton : PhoneSideButton
    {
        [Tooltip("사운드 바 나타나는 연출")] public MMF_Player appearMMFPlayer;

        public Sprite bgmSoundIcon;
        public Sprite effectSoundIcon;

        public override void Awake()
        {
            base.Awake();

            StartCoroutine(SoundCanvasInstantiateEnumerator());
        }

        private IEnumerator SoundCanvasInstantiateEnumerator()
        {
            while (phone.applicationControl.GetApp("Home") == null)
            {
                yield return null;
            }

            var rect = SoundManager.Instance.soundCanvasRectTransform;
            var phoneCanvas = PhoneUtil.GetPhoneCanvas(phone);
            rect.SetParent(phoneCanvas.transform, false);
            
            // 기존 프리셋 유지
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            PhoneUtil.SetLayer(rect);
        }
    }
}