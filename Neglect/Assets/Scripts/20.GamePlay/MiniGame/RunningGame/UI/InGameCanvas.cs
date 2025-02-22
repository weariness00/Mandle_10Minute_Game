using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.MiniGame.RunningGame
{
    public class InGameCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;
        
        [Header("Game Continue 관련")] 
        public Canvas continueCanvas;
        public TMP_Text countDownText;
        public List<Color> countDownTextColor = new List<Color>();
        [HideInInspector] public Action onGameStart;

        public void Awake()
        {
            continueCanvas.gameObject.SetActive(false);
        }

        // 게임 시작 카운트 다운
        public void GameContinueCountDown(float count = 3f)
        {
            continueCanvas.gameObject.SetActive(true);
            StartCoroutine(GameContinueEnumerator(count));
        }
        
        private IEnumerator GameContinueEnumerator(float count = 3f)
        {
            int startCount = (int)count + 1;
            while (true)
            {
                count -= Time.deltaTime;
                int value = ((int)Mathf.Round(count));
                countDownText.text = value.ToString();
                countDownText.color = countDownTextColor[startCount - (value + 1)];
                if (value <= 0)
                {
                    countDownText.text = "GO!";
                    break;
                }
                yield return null;
            }
            
            continueCanvas.gameObject.SetActive(false);
            onGameStart?.Invoke();
        }
    }
}

