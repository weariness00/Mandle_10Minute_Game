using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame.UI
{
    public class MatchingCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;

        public TMP_Text rankText; // 전체 랭킹
        public TMP_Text topScoreText; // 본인 점수 중 가장 높은 점수
        public Button gameStartButton;
        public TMP_Text gameStartButtonText;

        [Space] 
        public RunningGame runningGame;

        public void OnEnable()
        {
            var data = runningGame.CurrentPlayerData;
            rankText.text = $"현재 랭킹 : {(data.rank == 0 ? 2 : data.rank)}위";
            topScoreText.text = "최고 점수 : " + (PlayerPrefs.HasKey($"{nameof(RunningGame)}Score") ? PlayerPrefs.GetInt($"{nameof(RunningGame)}Score").ToString() : data.score.ToString());
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