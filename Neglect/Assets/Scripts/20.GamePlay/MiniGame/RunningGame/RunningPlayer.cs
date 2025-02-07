using System;
using Manager;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public partial class RunningPlayer : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rigidbody2D;
        
        public ReactiveProperty<int> life = new (3);
        public MinMaxValue<float> immortalTime = new(0, 0, 1); 
        
        [Tooltip("점프 높이")]public float jumpForce = 1f;
        [Tooltip("점프하는데 걸리는 시간")]public MinMaxValue<float> jumpTime = new(0,0,1f, false, true);
        public bool isJumping = false;
        public bool isSliding = false;
        private Vector3 originPosition;
        private Vector3 jumpDestinationPosition;

        public void Awake()
        {
            rigidbody2D = GetComponentInChildren<Rigidbody2D>();
            animator.animator = GetComponentInChildren<Animator>();
        }

        public void Update()
        {
            immortalTime.Current -= Time.deltaTime;
        }

        public void FixedUpdate()
        {
            Jump();
            Sliding();
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Running Ground"))
            {
                if (isJumping)
                {
                    animator.Randing();
                    isJumping = false;
                }
            }
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
            if(isSliding) return;
            
            if (isJumping && !jumpTime.IsMax)
            {
                jumpTime.Current += Time.deltaTime * RunningGame.GameSpeed;
                transform.position = Vector3.Lerp(originPosition, jumpDestinationPosition, Mathf.Sin(jumpTime.Current / jumpTime.Max * Mathf.PI));
            }
            else if (!isJumping && InputManager.running.MovePosition.y > 0f)
            {
                isJumping = true;
                jumpTime.SetMin();
                originPosition = transform.position;
                jumpDestinationPosition = transform.position + jumpForce * Vector3.up;
                animator.Jump();
            }
        }

        public void Sliding()
        {
            if(isJumping) return;
            
            if (!isSliding && InputManager.running.SlidingDown)
            {
                isSliding = true;
                animator.StartSliding();
            }
            else if (isSliding && !InputManager.running.SlidingDown)
            {
                isSliding = false;
                animator.EndSliding();
            }
        }
    }

    // 애니메이션 관련
    public partial class RunningPlayer
    {
        private RunningPlayerAnimator animator = new();

        [Serializable]
        public class RunningPlayerAnimator
        {
            public static implicit operator Animator(RunningPlayerAnimator value) => value.animator;

            [HideInInspector] public Animator animator;
            
            private static readonly int t_Jump = Animator.StringToHash("Jump");
            private static readonly int t_Randing = Animator.StringToHash("Randing");
            private static readonly int t_StartSliding = Animator.StringToHash("Start Sliding");
            private static readonly int t_EndSliding = Animator.StringToHash("End Sliding");
            
            public void Jump() => animator.SetTrigger(t_Jump);
            public void Randing() => animator.SetTrigger(t_Randing);
            public void StartSliding() => animator.SetTrigger(t_StartSliding);
            public void EndSliding() => animator.SetTrigger(t_EndSliding);
        }
    }
}

