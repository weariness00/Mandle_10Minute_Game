using GamePlay.MiniGame.FlappingGame;
using Quest;
using TMPro;
using UniRx;
using UnityEngine;

namespace SeoTestTestTest
{
    public class Seo_Test : MonoBehaviour
    {
        public int questId;

        public FlappingGameManager flappingGameManager;
        public TMP_Text scoreText;
        public void Start()
        {
            if (questId != -1)
            {
            }

            if (flappingGameManager)
            {
                if (scoreText) flappingGameManager.score.Subscribe(value => scoreText.text = $"{value}");
            }
        }
    }
}
