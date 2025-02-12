using GamePlay.Phone;
using System;
using Manager;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    // 멤버 변수
    // 유니티 이벤트 함수
    public partial class RunningGame : MiniGameBase
    {
        public static float GameSpeed = 1f;

        public GameObject runningGameObjectRoot;
        public Canvas runningGameCanvasRoot;

        [Header("Lobby 관련")] 
        public Canvas lobbyCanvas;
        public GameObject lobbyObject;
        
        [Header("In Game 관련")]
        public Canvas inGameCanvas;
        public GameObject inGameObject;
        
        public PlayerData[] playerDataArray = new PlayerData[3];
        public List<ObjectSpawner> obstacleSpawnerList;

        public override void Awake()
        {
            base.Awake();
            
            InputManager.running.input.Enable();
        }

        public override void Start()
        {
            base.Start();

            if (FindObjectOfType<PhoneControl>() != null)
            {
                foreach (ObjectSpawner spawner in obstacleSpawnerList)
                {
                    spawner.SpawnSuccessAction.AddListener(obj =>
                    {
                        obj.layer = LayerMask.NameToLayer("Phone");
                        SceneManager.MoveGameObjectToScene(obj, SceneUtil.GetRunningGameScene());
                    });
                }
            }
            
            gameSpeed.Subscribe(value =>
            {
                GameSpeed = value;
                foreach (ObjectSpawner spawner in obstacleSpawnerList)
                {
                    spawner.timeScale = value;
                }
            });
        }
    }

    
    public partial class RunningGame
    {
        public PlayerData GetPlayerData() => playerDataArray[0];

        [Serializable]
        public class PlayerData
        {
            public ReactiveProperty<int> score = new(0);
            public string name;
        }
        
        public override void GamePlay()
        {
            base.GamePlay();
            lobbyCanvas.gameObject.SetActive(false);
            lobbyObject.gameObject.SetActive(false);
            
            inGameCanvas.gameObject.SetActive(true);
            inGameObject.gameObject.SetActive(true);
        }
    }

    public partial class RunningGame
    {
        public override void AppInstall()
        {
            base.AppInstall();
            runningGameObjectRoot.SetActive(false);
            runningGameCanvasRoot.gameObject.SetActive(false);
        }

        public override void AppPause()
        {
            base.AppPause();
            GameStop();
        }

        public override void AppPlay()
        {
            base.AppPlay();
            runningGameObjectRoot.SetActive(true);
            runningGameCanvasRoot.gameObject.SetActive(true);
        }
    }
}

