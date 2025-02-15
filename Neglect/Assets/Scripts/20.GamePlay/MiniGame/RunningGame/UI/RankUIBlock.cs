using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame
{
    public class RankUIBlock : MonoBehaviour
    {
        public RunningGame.PlayerData data;

        public Image rankIcon;
        public TMP_Text rankText;
        public TMP_Text nameText;
        public TMP_Text scoreText;
        [HideInInspector] public RectTransform rectTransform;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}

