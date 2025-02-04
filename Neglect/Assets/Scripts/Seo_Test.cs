using GamePlay.MiniGame.FlappingGame;
using Quest;
using TMPro;
using UniRx;
using UnityEngine;

namespace SeoTestTestTest
{
    public class Seo_Test : MonoBehaviour
    {
        public QuestBase quest;

        public FlappingGameManager flappingGameManager;
        public TMP_Text scoreText;
        public void Start()
        {
            if(quest) quest.Play();

            if (flappingGameManager)
            {
                if (scoreText) flappingGameManager.score.Subscribe(value => scoreText.text = $"{value}");
            }
        }
    }
}
