using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame.UI
{
    public class MatchingCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;

        public TMP_Text topScoreText; // 본인 점수 중 가장 높은 점수
        public Button gameStartButton;

        [Space] 
        public RunningGame runningGame;

        public void OnEnable()
        {
            var curScore = runningGame.CurrentPlayerData.score.ToString();
            topScoreText.text = "최고 점수 : " + (PlayerPrefs.HasKey($"{nameof(RunningGame)}Score") ? PlayerPrefs.GetInt($"{nameof(RunningGame)}Score").ToString() : curScore);
        }

        public void OnDestroy()
        {
            var curScore = runningGame.CurrentPlayerData.score.Value;
            if (PlayerPrefs.HasKey($"{nameof(RunningGame)}Score") && PlayerPrefs.GetInt($"{nameof(RunningGame)}Score") > curScore)
                curScore = PlayerPrefs.GetInt($"{nameof(RunningGame)}Score");
            PlayerPrefs.SetInt($"{nameof(RunningGame)}Score", curScore);
        }
    }
}