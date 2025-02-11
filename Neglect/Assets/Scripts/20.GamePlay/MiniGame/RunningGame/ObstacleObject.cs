using System;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    public class ObstacleObject : MonoBehaviour
    {
        public float speed = 1f;
        [Tooltip("장애물을 피할 시 주는 추가 점수")]public int extraScore = 10;
        [Tooltip("플레이어가 성공적으로 피했을때")]public bool isCollision = false;
        
        public void Update()
        {
            transform.position += Time.deltaTime * RunningGame.GameSpeed * speed * Vector3.left;
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            // 플레이어와 충돌하지 않았을때만 점수 추가
            if (other.CompareTag("Running Score Line"))
            {
                if (!isCollision)
                {
                    var runningGame = FindObjectOfType<RunningGame>();
                    if (runningGame != null)
                    {
                        runningGame.GetPlayerData().score.Value += extraScore;
                    }
                }
                Destroy(gameObject, 2f);
            }
        }
    }
}

