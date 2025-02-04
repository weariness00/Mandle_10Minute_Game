using UnityEngine;

namespace GamePlay.MiniGame.FlappingGame
{
    public class WallObject : MonoBehaviour
    {
        public float speed = 1f;
        public int extraScore = 1;
        
        private FlappingGameManager flappingGameManager;

        private void Awake()
        {
            flappingGameManager = FindObjectOfType<FlappingGameManager>();
        }

        public void Update()
        {
            if(flappingGameManager.isGamePlay.Value == false) return;
            
            transform.position += speed * flappingGameManager.gameSpeed.Value * Time.deltaTime * Vector3.left;
        }
    }
}

