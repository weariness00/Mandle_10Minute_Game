using GamePlay.MiniGame;
using System.Collections.Generic;
using UniRx;
using Util;

namespace GamePlay.MiniGame.FlappingGame
{
    public class FlappingGameManager : MiniGameBase
    {
        public ReactiveProperty<int> score = new(0);
        public ObjectSpawner spawner;

        public override void GamePlay()
        {
            base.GamePlay();
            spawner.Play();
        }

        public override void GameStop()
        {
            base.GameStop();
            spawner.Pause();
        }

        public override void GameOver()
        {
            base.GameOver();
            spawner.Stop();
        }
    }
}

