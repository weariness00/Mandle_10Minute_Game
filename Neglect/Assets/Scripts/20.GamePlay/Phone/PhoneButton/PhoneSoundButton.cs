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
        public Slider sliderPrefab;
        
        private Slider slider;
        private RectTransform sliderRectTransform;
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

            slider = PhoneUtil.InstantiateUI(sliderPrefab);
            slider.onValueChanged.AddListener(value =>
            {
                disposableDisposable?.Dispose();
                disposableDisposable = Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => DisappearSlider());
                SoundManager.Instance.SetVolume("Master", value);
            });

            sliderRectTransform = slider.GetComponent<RectTransform>();
            sliderRectTransform.anchoredPosition = new Vector2(sliderRectTransform.sizeDelta.y, 0);
            parent = slider.transform.parent;
        }

        public void AppearSlider()
        {
            tween?.Kill();
            tween = sliderRectTransform.DOAnchorPos(new Vector2(-40, 0), 0.3f).SetEase(Ease.Flash);
        }
        public void DisappearSlider()
        {
            tween?.Kill();
            tween = sliderRectTransform.DOAnchorPos(new Vector2(sliderRectTransform.sizeDelta.y, 0), 0.3f).SetEase(Ease.Flash);
        }
        
        public void SoundUp()
        {
            AppearSlider();
            slider.value += 10;
        }

        public void SoundDown()
        {
            AppearSlider();
            slider.value -= 10;
        }
    }
}