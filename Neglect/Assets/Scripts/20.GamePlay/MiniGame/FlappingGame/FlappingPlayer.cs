using Manager;
using UniRx;
using UnityEngine;

namespace GamePlay.MiniGame.FlappingGame
{
    public class FlappingPlayer : MonoBehaviour
    {
        public float jumpPower = 1f;
        
        private FlappingGameManager gameManager;
        private Rigidbody2D rigidbody2D;
        private Vector2 originVelocity;

        public void Awake()
        {
            gameManager = FindObjectOfType<FlappingGameManager>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Start()
        {
            gameManager.isGamePlay.Subscribe(value =>
            {
                if (value)
                {
                    rigidbody2D.gravityScale = gameManager.gameSpeed.Value;
                    rigidbody2D.velocity = originVelocity;
                }
                else
                {
                    originVelocity = rigidbody2D.velocity;
                    rigidbody2D.velocity = Vector2.zero;
                    rigidbody2D.gravityScale = 0;
                }
            });
            gameManager.gameSpeed.Subscribe(value => rigidbody2D.gravityScale = value);
        }

        public void FixedUpdate()
        {
            if(gameManager.isGamePlay.Value == false) return;
            
            if (InputManager.flapping.IsJump)
            {
                rigidbody2D.AddForce(jumpPower * gameManager.gameSpeed.Value * Vector2.up);
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Flapping Obstacle"))
            {
                gameManager.GameOver();
            }
            else if (other.CompareTag("Flapping Score Line"))
            {
                var wall = other.GetComponentInParent<WallObject>();
                gameManager.score.Value += wall.extraScore;
            }
        }
    }
}

