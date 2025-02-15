using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    public class GroundObject : MonoBehaviour
    {
        public RunningGame runningGame;
        // Start is called before the first frame update
        public float MoveSpeed;
      
        public void Setting(float speed)
        {
            MoveSpeed = speed;
        }
        void Update()
        {
            if(!runningGame.isGamePlay.Value) return;
            transform.position += Time.deltaTime * runningGame.gameSpeed.Value * MoveSpeed * Vector3.left;
        }
    }
}