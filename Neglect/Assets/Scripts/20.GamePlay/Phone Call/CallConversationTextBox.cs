using DG.Tweening;
using KoreanTyper;
using Manager;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Util;

namespace GamePlay.Event
{
    public class CallconversationTextBox : MonoBehaviour
    {


        public AudioClip typingSound;
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
                var audioSource = SoundManager.Instance.GetAudioSource("Effect");
                audioSource.PlayOneShot(typingSound);
                if (narrationReadTimer.IsMax)
                {
                     isEndAnimation?.Invoke();
                }
            }
        }
        string InsertNewlinesEveryNChars(string text, int n)
        {
            if (string.IsNullOrEmpty(text) || n <= 0) return text;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                sb.Append(text[i]);
                if ((i + 1) % n == 0 && i != text.Length - 1)
                {
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }
        public void SetNarration(string narration , int n , Action isEndAnimationUse)
        {
            isEndAnimation = null;
            isEndAnimation += isEndAnimationUse;
            narrationSTR = InsertNewlinesEveryNChars(narration, n);
            narrationReadTimer.SetMin();
        }
    }
}