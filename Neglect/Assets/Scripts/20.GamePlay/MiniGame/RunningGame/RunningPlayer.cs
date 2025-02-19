using System;
using Manager;
using MoreMountains.Feedbacks;
using UniRx;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public partial class RunningPlayer : MonoBehaviour
    {
        public RunningGame runningGame;
        [HideInInspector] public Rigidbody2D rigidbody2D;
        [HideInInspector] public BoxCollider2D collider2D;
        
        public MinMaxValue<float> immortalTime = new(0, 0, 1);
        public MMF_Player hitEffect;
        public AudioSource audioSource;
        
        [Space]
        [Header("체력 관련")]
        public ReactiveProperty<int> life = new (5);
        public int lifeMax = 5;
        public MinMaxValue<int> healCounting = new(0, 0, 5);
        
        [Space]
        [Header("Jump 관련")]
        [Tooltip("점프 높이")]public float jumpForce = 1f;
        [Tooltip("점프하는데 걸리는 시간")]public MinMaxValue<float> jumpTime = new(0,0,1f, false, true);
        public bool isJumping = false;

        [Space] 
        public AudioClip randingClip;
        
        [Space]
        [Tooltip("슬라이딩 충돌 박스 크기")] public Vector2 slidingColliderBoxSize;
        public bool isSliding = false;
        
        private Vector3 originPosition;
        private Vector3 jumpDestinationPosition;
        private Vector2 originColliderSize;
        private Vector2 originCollideroffset;

        public void Awake()
        {
            rigidbody2D = GetComponentInChildren<Rigidbody2D>();
            collider2D = GetComponentInChildren<BoxCollider2D>();
            animator.animator = GetComponentInChildren<Animator>();
        }

        public void Start()
        {
            originColliderSize = collider2D.size;
            originCollideroffset = collider2D.offset;

            isJumping = true;
            jumpTime.SetMax();
        }

        public void Update()
        {
            if(!runningGame.isGamePlay.Value) return;
            immortalTime.Current -= Time.deltaTime * runningGame.gameSpeed.Value;
        }

        public void FixedUpdate()
        {
            if(!runningGame.isGamePlay.Value) return;
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
                    audioSource.PlayOneShot(randingClip);
                }
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (immortalTime.IsMin && other.CompareTag("Running Obstacle"))
            {
                var obstacle = other.GetComponent<RunningObstacle>();
                obstacle.isCollision = true;
                immortalTime.SetMax();
                life.Value--;
                healCounting.SetMin();
                if(hitEffect) hitEffect.PlayFeedbacks();
            }
        }
        
        public void Jump()
        {
            if(isSliding) return;
            
            if (isJumping && !jumpTime.IsMax)
            {
                jumpTime.Current += Time.deltaTime * runningGame.gameSpeed.Value;
                transform.position = Vector3.Lerp(originPosition, jumpDestinationPosition, Mathf.Sin(jumpTime.Current / jumpTime.Max * Mathf.PI));
            }
            else if (isJumping)
            {
                transform.position -= jumpForce / jumpTime.Max * Time.deltaTime * runningGame.gameSpeed.Value * Vector3.up;
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
                collider2D.size = slidingColliderBoxSize;
                collider2D.offset = new Vector2(originCollideroffset.x, originCollideroffset.y -(originColliderSize.y - slidingColliderBoxSize.y) / 2f);
                
                isSliding = true;
                animator.StartSliding();
            }
            else if (isSliding && !InputManager.running.SlidingDown)
            {
                collider2D.size = originColliderSize;
                collider2D.offset = originCollideroffset;
                
                isSliding = false;
                animator.EndSliding();
            }
        }

        public void Healing(int count)
        {
            var value = life.Value + count;
            if (value > lifeMax) value = lifeMax;
            else if (value < 0) value = 0;
            life.Value = value;
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

            public void Jump()
            {
                animator.ResetTrigger(t_Randing);
                animator.SetTrigger(t_Jump);
            }
            public void Randing() => animator.SetTrigger(t_Randing);

            public void StartSliding()
            {
                animator.ResetTrigger(t_EndSliding);
                animator.SetTrigger(t_StartSliding);
            }
            public void EndSliding() => animator.SetTrigger(t_EndSliding);
        }
    }
}

