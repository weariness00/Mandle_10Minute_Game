using DG.Tweening;
using Manager;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Phone
{
    public class PhoneSoundButton : PhoneSideButton
    {
        public SoundBlock soundBlockPrefab;

        private bool isInit = false;
        private SoundBlock soundBlock;
        private RectTransform blockectTransform;
        private Transform parent;
        private Tween tween;
        private IDisposable disposableDisposable;

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

            soundBlock = PhoneUtil.InstantiateUI(soundBlockPrefab);
            soundBlock.slider.value = SoundManager.Instance.GetVolume("Master");
            blockectTransform = soundBlock.GetComponent<RectTransform>();
            blockectTransform.anchoredPosition = new Vector2(blockectTransform.sizeDelta.y, 0);
            parent = soundBlock.slider.transform.parent;
            
            soundBlock.slider.onValueChanged.AddListener(value =>
            {
                disposableDisposable?.Dispose();
                disposableDisposable = Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => DisappearSlider());
                SoundManager.Instance.SetVolume("Master", value);
            });

            isInit = true;
        }

        public void AppearSlider()
        {
            if(!isInit) return;
            tween?.Kill();
            tween = blockectTransform.DOAnchorPos(new Vector2(-40, 0), 0.3f).SetEase(Ease.Flash);
        }
        public void DisappearSlider()
        {
            if(!isInit) return;
            tween?.Kill();
            tween = blockectTransform.DOAnchorPos(new Vector2(blockectTransform.sizeDelta.y, 0), 0.3f).SetEase(Ease.Flash);
        }
        
        public void SoundUp()
        {
            AppearSlider();
            soundBlock.slider.value += 10;
        }

        public void SoundDown()
        {
            AppearSlider();
            soundBlock.slider.value -= 10;
        }
    }
}