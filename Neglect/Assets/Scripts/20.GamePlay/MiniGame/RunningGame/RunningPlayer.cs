using System;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay.MiniGame.RunningGame
{
    public class RunningPlayer : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rigidbody2D;

        public float jumpForce = 1f;
        public bool isJumping = false;
        
        public void Awake()
        {
            rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        }

        public void FixedUpdate()
        {
            Jump();
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            isJumping = false;
        }

        public void Jump()
        {
            if(isJumping) return;
            if (InputManager.running.MovePosition.y > 0f)
            {
                Debug.Log("점프");
                isJumping = true;
                rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}

