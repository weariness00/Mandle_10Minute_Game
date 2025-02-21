using DG.Tweening;
using KoreanTyper;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Util;

namespace GamePlay
{
    public class GamePlayerNarration : MonoBehaviour
    {
        public GameObject narrationObject; // 나레이션 오브젝트
        public CanvasGroup canvasGroup; // 알파 값 사용용도
        public TMP_Text narrationText; // 나레이션 텍스트
        public MinMaxValue<float> narrationReadTimer = new(0,0,1); // 나레이션 읽는 속도
        public float nextNarrationSettingDuration = 2f;
        public string narrationSTR;

        [TextArea]
        [Tooltip("나레이션 순서")]public List<string> narrationList;
        private int narrationIndex = 0;
        
        public void Awake()
        {
            narrationReadTimer.SetMax();
            SetNarration(narrationList[narrationIndex++]);
        }

        public void Update()
        {
            if (!narrationReadTimer.IsMax)
            {
                narrationReadTimer.Current += Time.deltaTime;
                narrationText.text = narrationSTR.Typing(narrationReadTimer.NormalizeToRange());
                if (narrationReadTimer.IsMax)
                {
                    DOVirtual.DelayedCall(nextNarrationSettingDuration, () =>
                    {
                        if (narrationIndex < narrationList.Count)
                            SetNarration(narrationList[narrationIndex++]);
                        else
                            canvasGroup.DOFade(0,4f).OnComplete(() => narrationObject.SetActive(true));
                    });
                }
            }
        }

        public void SetNarration(string narration)
        {
            narrationSTR = narration;
            narrationReadTimer.SetMin();
            canvasGroup.alpha = 1;
            narrationObject.SetActive(true);
        }
    }
}

