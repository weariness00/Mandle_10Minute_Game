using System;
using UnityEngine;

namespace Util
{
    public class UIBouncingPosition : MonoBehaviour
    {
        [Tooltip("해당 부모의 범위 내에서 움직인다.")]public RectTransform parentRectTransform;
        [Tooltip("움직일 스피드")] public float speed = 1;
        [Tooltip("움직이기전 대기 시간")] public float startFirstDuration = 2;
        [Tooltip("다 움직이고 나서 대기 시간")] public float stayMoveDuration = 2;
        
        private RectTransform _rectTransform;
        private Vector3 originLocalPosition;
        private MinMaxValue<float> stayFirstTimer;
        private MinMaxValue<float> stayMoveTimer;

        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            originLocalPosition = _rectTransform.localPosition;
            stayFirstTimer = new(0, 0, startFirstDuration);
            stayMoveTimer = new(0, 0, stayMoveDuration);
        }

        public void Update()
        {
            if (!stayFirstTimer.IsMax)
            {
                stayFirstTimer.Current += Time.deltaTime;
                return;
            }
            if (_rectTransform.sizeDelta.x > parentRectTransform.sizeDelta.x)
            {
                if (_rectTransform.sizeDelta.x + _rectTransform.localPosition.x < parentRectTransform.sizeDelta.x)
                {
                    stayMoveTimer.Current += Time.deltaTime;
                    if (stayMoveTimer.IsMax)
                    {
                        stayFirstTimer.SetMin();
                        stayMoveTimer.SetMin();
                        _rectTransform.localPosition = originLocalPosition;
                    }
                }
                else
                {
                    _rectTransform.localPosition += Time.deltaTime * speed * Vector3.left; // x축의 왼쪽으로 이동
                }
            }
        }
    }
}

