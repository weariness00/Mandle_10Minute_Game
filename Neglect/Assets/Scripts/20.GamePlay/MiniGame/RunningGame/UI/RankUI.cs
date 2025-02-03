using UniRx;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    public class RankUI : MonoBehaviour
    {
        public RankUIBlock dataBlockPrefab;
        
        public void Start()
        {
            RunningGame runningGame = FindObjectOfType<RunningGame>();
            foreach (var playerData in runningGame.playerDataArray)
            {
                var block = Instantiate(dataBlockPrefab, transform);
                playerData.score.Subscribe(value => block.scoreText.text = $"{value}pt");
            }
        }
    }
}

