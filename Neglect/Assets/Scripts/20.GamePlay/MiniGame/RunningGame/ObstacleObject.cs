using System;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    public class ObstacleObject : MonoBehaviour
    {
        public float speed = 1f;
        
        public void Update()
        {
            transform.position += Time.deltaTime * RunningGame.GameSpeed * speed * Vector3.left;
        }
    }
}

