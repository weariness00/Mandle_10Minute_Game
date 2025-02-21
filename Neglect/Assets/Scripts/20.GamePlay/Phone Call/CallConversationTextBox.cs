using DG.Tweening;
using KoreanTyper;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace GamePlay.Event
{
    public class CallconversationTextBox : MonoBehaviour
    {
        
        public TMP_Text narrationText; // 나레이션 텍스트
        public MinMaxValue<float> narrationReadTimer = new(0, 0, 1); // 나레이션 읽는 속도
        public string narrationSTR;
        public Action isEndAnimation;

        public void Update()
        {
            if (!narrationReadTimer.IsMax)
            {
                narrationReadTimer.Current += Time.deltaTime;
                narrationText.text = narrationSTR.Typing(narrationReadTimer.NormalizeToRange());
                if (narrationReadTimer.IsMax)
                {
                    isEndAnimation?.Invoke();
                }
            }
        }
        public void SetNarration(string narration)
        {
            narrationSTR = narration;
            narrationReadTimer.SetMin();
        }
    }
}