using GamePlay.Phone;
using System;
using Manager;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    // 멤버 변수
    // 유니티 이벤트 함수
    public partial class RunningGame : MiniGameBase
    {
        public GameObject runningGameObjectRoot;
        public Canvas runningGameCanvasRoot;

        [Header("Setting 관련")] 
        public Canvas settingCanvas;
        public Button continueButton;
        public Button exitButton;
        
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

            settingCanvas.gameObject.SetActive(false);
            continueButton.onClick.AddListener(() =>
            {
                settingCanvas.gameObject.SetActive(false);
                if(isGameStart) isGamePlay.Value = true;
            });
            
            InputManager.running.ESC.performed += context =>
            {
                settingCanvas.gameObject.SetActive(!settingCanvas.gameObject.activeSelf);

                if (isGameStart)
                {
                    if (isGamePlay.Value) GameStop();
                    else  GamePlay();
                }
            };
            
            foreach (ObjectSpawner spawner in obstacleSpawnerList)
            {
                spawner.SpawnSuccessAction.AddListener(obj =>
                {
                    obj.layer = LayerMask.NameToLayer("Phone");
                    SceneManager.MoveGameObjectToScene(obj, SceneUtil.GetRunningGameScene());
                    obj.GetComponent<ObstacleObject>().runningGame = this;
                });
            }
            
            isGamePlay.Subscribe(value =>
            {
                if (value)
                {
                    foreach (ObjectSpawner spawner in obstacleSpawnerList)
                        spawner.Play();
                }
                else
                {
                    foreach (ObjectSpawner spawner in obstacleSpawnerList)
                        spawner.Pause();
                }
            });
            
            gameSpeed.Subscribe(value =>
            {
                foreach (ObjectSpawner spawner in obstacleSpawnerList)
                    spawner.timeScale = value;
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
        public override void AppInstall(PhoneControl phone)
        {
            base.AppInstall(phone);
            runningGameObjectRoot.SetActive(false);
            runningGameCanvasRoot.gameObject.SetActive(false);
            
            // 나가기 누르면 앱으로 이동 ( 게임은 종료되지 않음 )
            exitButton.onClick.AddListener(() =>
            {
                phone.applicationControl.OnHome();
            });
        }

        public override void AppPlay(PhoneControl phone)
        {
            base.AppPlay(phone);
            runningGameObjectRoot.SetActive(true);
            runningGameCanvasRoot.gameObject.SetActive(true);
        }

        public override void AppResume(PhoneControl phone)
        {
            base.AppResume(phone);
            runningGameObjectRoot.SetActive(true);
            runningGameCanvasRoot.gameObject.SetActive(true);
            InputManager.running.input.Enable();
            if(isGameStart) GamePlay();
        }

        public override void AppPause(PhoneControl phone)
        {
            base.AppPause(phone);
            runningGameObjectRoot.SetActive(false);
            runningGameCanvasRoot.gameObject.SetActive(false);
            InputManager.running.input.Disable();
            GameStop();
        }
    }
}

