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
            
            foreach (ObjectSpawner spawner in obstacleSpawnerList)
            {
                spawner.SpawnSuccessAction.AddListener(obj =>
                {
                    obj.layer = LayerMask.NameToLayer("Phone");
                    SceneManager.MoveGameObjectToScene(obj, SceneUtil.GetRunningGameScene());
                });
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
    }

    public partial class RunningGame
    {
        [Serializable]
        public class PlayerData
        {
            public ReactiveProperty<int> score = new(0);
            public string name;
        }
    }

    
}

