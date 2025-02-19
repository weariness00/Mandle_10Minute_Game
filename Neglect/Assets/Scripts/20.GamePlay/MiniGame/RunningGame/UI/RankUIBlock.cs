using TMPro;
using UniRx;
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

        public void Start()
        {
            if (data != null)
            {
                data.mainColor.Subscribe(value => rankIcon.color = value);
            }
        }
    }
}

