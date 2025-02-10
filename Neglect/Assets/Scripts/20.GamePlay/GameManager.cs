using GamePlay;
using GamePlay.MiniGame;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public MiniGameBase currentMiniGame;
        public MMF_Player test1;
        public MMF_Player test2;
    
        public void Awake()
        {
            SceneUtil.AsyncAddPhone();
        
            // 테스트용
            SceneUtil.AsyncAddRunningGame(scene =>
            {
                // 씬 로드 되면 일단 미니게임 초기화 하고 멈추기
                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                {
                    var miniGameManager = rootGameObject.GetComponentInChildren<MiniGameBase>();
                    if (miniGameManager != null)
                    {
                        miniGameManager.InitLoadedScene(scene);
                        currentMiniGame = miniGameManager;
                        break;
                    }
                }
            });
        }

        // 테스트용
        public void MiniGameOnOff()
        {
            if (currentMiniGame != null)
            {
                if (!currentMiniGame.isGamePlay.Value)
                {
                    currentMiniGame.GamePlay();
                    test1.PlayFeedbacks();
                }
                else
                {
                    currentMiniGame.GameStop();
                    test2.PlayFeedbacks();
                }
            }
        }
    }
}

