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
        public SpriteRenderer Renderer;


        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();    
        }
        public void Setting(float speed)
        {
            MoveSpeed = speed;
        }
        public void Setting(float speed , int FarLevel)
        {
            Color pre = Renderer.color;
            if (FarLevel == 0)
            {
                Renderer.color = new Color(1f, 1f, 1f, 150/255f);
            }
            if (FarLevel == 1)
            {
                Renderer.color = new Color(1f,1f,1f,1f);
            }
            if (FarLevel == 2)
            {
                Renderer.color = new Color(240 / 255f, 240 / 255f, 240 / 255f, 1f);
            }
            Setting(speed);
        }
        void Update()
        {
            if(!runningGame.isGamePlay.Value) return;
            transform.position += Time.deltaTime * runningGame.gameSpeed.Value * MoveSpeed * Vector3.left;
        }
    }
}