using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    public class RunningObstacle : MonoBehaviour
    {
        [HideInInspector] public RunningGame runningGame;

        public float speed = 1f;
        [Tooltip("장애물을 피할 시 주는 추가 점수")]public int extraScore = 10;
        [Tooltip("플레이어가 성공적으로 피했을때")]public bool isCollision = false;
        
        public void Update()
        {
            if(!runningGame.isGamePlay.Value) return;
            transform.position += Time.deltaTime * runningGame.gameSpeed.Value * speed * Vector3.left;
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

