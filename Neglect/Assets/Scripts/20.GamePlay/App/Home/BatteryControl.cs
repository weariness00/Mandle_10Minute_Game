
using DG.Tweening;
using Quest;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Random = UnityEngine.Random;

namespace GamePlay.App
{
    public class BatteryControl : MonoBehaviour
    {
        [Tooltip("배터리 이미지")] public Image image;
        [Tooltip("배터리 감소에 걸리는 시간")] public float decreaseTime = 60;
        [Tooltip("배터리가 감소되는 양")]public float decreaseAmount = 5;

        public ReactiveProperty<float> currentAmount = new(100);
        [Tooltip("배터리가 랜덤으로 생성될 경우의 배터리 용량 원소")]public List<float> batteryRandomElement = new List<float>(){100,70,50,30};

        public bool isDecreaseBattery = false;

        private IDisposable onBatteryEventDisposable; // 배터리 이벤트는 1번만 동작해야되서 사용
        public void Awake()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.isGameStart.Subscribe(value =>
                {
                    var index = Random.Range(0, batteryRandomElement.Count);
                    currentAmount.Value = batteryRandomElement[index];
                    image.DOFillAmount(batteryRandomElement[index] / 100f, 1f).OnComplete(() => isDecreaseBattery = value);
                });

                onBatteryEventDisposable = currentAmount.Subscribe(value =>
                {
                    if (value <= 10)
                    {
                        var quest = QuestDataList.Instance.InstantiateEvent(GameManager.Instance.batteryEventID);
                        if (quest) quest.Play();
                        
                        onBatteryEventDisposable?.Dispose();
                    }
                });
            }
        }

        public void Update()
        {
            if (isDecreaseBattery)
            {
                currentAmount.Value -= decreaseAmount * Time.deltaTime / decreaseTime;
                image.fillAmount = currentAmount.Value / 100f;
            }
        }
    }
}

