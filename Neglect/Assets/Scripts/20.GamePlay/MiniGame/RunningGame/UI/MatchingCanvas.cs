using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame.UI
{
    public class MatchingCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;

        [Header("Player Information 관련")] 
        public Image rankIcon;
        public List<Sprite> rankSpriteList;
        public TMP_Text rankText; // 전체 랭킹
        public TMP_Text topScoreText; // 본인 점수 중 가장 높은 점수

        [Header("Matching Start Information 관련")]
        public TMP_Text seasonText;
        public TMP_Text warringText;
        public Button gameStartButton;
        public TMP_Text gameStartButtonText;

        [Header("Match Loading 관련")] 
        public GameObject matchLoadingObject;
        public TMP_Text matchCheckText; // 매칭중인지 알려주는 텍스트
        public TMP_Text matchTimeText; // 매칭 시간 알려주는 텍스트
        [Tooltip("몇초 뒤에 매칭이 완료됬다고 할 것인지")] public float whenMatchedDuration;

        [HideInInspector] public UnityEvent onMatchedEvent = new();
        private float matchTimer = 0f;
        
        [Space] 
        public RunningGame runningGame;

        public void Update()
        {
            if (matchLoadingObject.activeSelf)
            {
                // mm:ss로 표시
                matchTimer += Time.deltaTime;
                var t = TimeSpan.FromSeconds(matchTimer);
                matchTimeText.text = $"{(int)t.TotalMinutes:D2} : {t.Seconds:D2}";
                if (matchTimer > whenMatchedDuration)
                {
                    matchCheckText.text = "매칭 완료!";
                    matchTimeText.text = "";
                }
                // 매칭 완료
                if (matchTimer > whenMatchedDuration + 1)
                {
                    onMatchedEvent?.Invoke();
                }
            }
        }

        public void OnEnable()
        {
            matchLoadingObject.SetActive(false);
            matchCheckText.text = "매칭 중";
            matchTimer = 0f;

            var data = runningGame.CurrentPlayerData;
            var hasRank = data.rank != 0;
            rankIcon.sprite = rankSpriteList[hasRank ? data.rank - 1 : 1];
            rankText.text = $"현재 랭킹 : {(hasRank ? data.rank : 2)}위";
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