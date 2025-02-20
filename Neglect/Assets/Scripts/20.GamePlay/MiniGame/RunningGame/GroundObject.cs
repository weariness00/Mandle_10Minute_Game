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
        public int groundType; // 구름 중에 흐린 구름의 갯수를 유지하기 위한 변수 추가
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