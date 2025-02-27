using DG.Tweening;
using GamePlay.Chatting;
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
        public AudioSource TalkSound;
        public ChatTextBox narrationTextBox; // 나레이션 텍스트
        public MinMaxValue<float> narrationReadTimer = new(0, 0, 1); // 나레이션 읽는 속도
        public string narrationSTR;
        public Action isEndAnimation;
        
        public void Update()
        {
            if (!narrationReadTimer.IsMax)
            {
                narrationReadTimer.Current += Time.deltaTime;
                narrationTextBox.SetText(narrationSTR.Typing(narrationReadTimer.NormalizeToRange()));
                if (narrationReadTimer.IsMax)
                {
                     isEndAnimation?.Invoke();
                }
            }
        }
        public void SetNarration(string narration, Action isEndAnimationUse)
        {
            TalkSound.Play();
            isEndAnimation = null;
            isEndAnimation += isEndAnimationUse;
            narrationTextBox.SetText("");
            narrationSTR = narration;
            narrationReadTimer.SetMin();
        }
    }
}