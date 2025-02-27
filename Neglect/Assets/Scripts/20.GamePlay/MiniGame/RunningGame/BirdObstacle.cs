using System.Collections.Generic;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public class BirdObstacle : RunningObstacle
    {
        public List<AnimationCurve> curveList;
        public CurveMovement2D moveCurve;

        public void Start()
        {
            transform.position = new Vector3(transform.position.x, -0.4f, transform.position.z);
            moveCurve.curve = curveList.Random();
            moveCurve.Init();
        }

        public new void Update()
        {
            if (runningGame.isGamePlay.Value)
            {
                moveCurve.speed = runningGame.gameSpeed.Value;
                moveCurve.Move();
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            // 플레이어와 충돌하지 않았을때만 점수 추가
            if (other.CompareTag("Running Score Line"))
            {
                if (!isCollision)
                {
                    runningGame.player.currentCombo.Value++;
                    runningGame.CurrentPlayerData.score.Value += extraScore * runningGame.player.GetComboMultiple();
                    runningGame.player.healCounting.Current++;
                    if (runningGame.player.healCounting.IsMax)
                    {
                        runningGame.player.healCounting.SetMin();
                        runningGame.player.Healing(1);
                    }
                }

                Destroy(gameObject, 2f);
            }
        }
    }
}