using System;
using Manager;
using UniRx;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public class RunningPlayer : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rigidbody2D;
        
        public ReactiveProperty<int> life = new (3);
        public MinMaxValue<float> immortalTime = new(0, 0, 1); 
        
        [Tooltip("점프 높이")]public float jumpForce = 1f;
        [Tooltip("점프하는데 걸리는 시간")]public MinMaxValue<float> jumpTime = new(0,0,1f, false, true);
        public bool isJumping = false;
        private Vector3 originPosition;
        private Vector3 jumpDestinationPosition;
        
        public void Awake()
        {
            rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        }

        public void Update()
        {
            immortalTime.Current -= Time.deltaTime;
        }

        public void FixedUpdate()
        {
            Jump();
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            if(other.gameObject.CompareTag("Running Ground"))
                isJumping = false;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (immortalTime.IsMin && other.CompareTag("Running Obstacle"))
            {
                var obstacle = other.GetComponent<ObstacleObject>();
                obstacle.isCollision = true;
                immortalTime.SetMax();
                life.Value--;
            }
        }
        
        public void Jump()
        {
            if (isJumping && !jumpTime.IsMax)
            {
                jumpTime.Current += Time.deltaTime * RunningGame.GameSpeed;
                transform.position = Vector3.Lerp(originPosition, jumpDestinationPosition, Mathf.Sin(jumpTime.Current / jumpTime.Max * Mathf.PI));
            }
            else if (!isJumping && InputManager.running.MovePosition.y > 0f)
            {
                Debug.Log("점프");
                isJumping = true;
                jumpTime.SetMin();
                originPosition = transform.position;
                jumpDestinationPosition = transform.position + jumpForce * Vector3.up;
            }
        }
    }
}

