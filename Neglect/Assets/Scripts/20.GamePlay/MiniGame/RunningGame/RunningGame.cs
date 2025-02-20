using GamePlay.Phone;
using System;
using Manager;
using Quest;
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
        [Space]
        [Header("Running Game")]
        public RunningPlayer player;
        public GameObject runningGameObjectRoot;
        public Canvas runningGameCanvasRoot;

        public int rankEventID;
        private QuestBase rankQuest;

        [Header("Setting 관련")] 
        public Canvas settingCanvas;
        public Button continueButton;
        public Button exitButton;
        
        [Header("Lobby 관련")] 
        public Canvas lobbyCanvas;
        public GameObject lobbyObject;
        public Button lobbyExitButton;
        
        [Header("In Game 관련")]
        public Canvas inGameCanvas;
        public GameObject inGameObject;
        
        public List<ObjectSpawner> obstacleSpawnerList;

        public override void Awake()
        {
            base.Awake();
            InputManager.running.input.Enable();

            settingCanvas.gameObject.SetActive(false);
            
            lobbyCanvas.gameObject.SetActive(true);
            lobbyObject.gameObject.SetActive(true);
            
            inGameCanvas.gameObject.SetActive(false);
            inGameObject.gameObject.SetActive(false);
            
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
                    PhoneUtil.SetLayer(obj);
                    SceneManager.MoveGameObjectToScene(obj, SceneUtil.GetRunningGameScene());
                    obj.GetComponent<RunningObstacle>().runningGame = this;
                    obj.transform.SetParent(inGameObject.transform);
                });
            }
            
            gameSpeed.Subscribe(value =>
            {
                foreach (ObjectSpawner spawner in obstacleSpawnerList)
                    spawner.timeScale = value;
            });
        }

        public override void Update()
        {
            base.Update();

            if (isGamePlay.Value)
            {
                UpdatePlayerData();
            }
        }
    }

    /// <summary>
    /// 자신과 다른 플레이어 Data 관리
    /// </summary>
    public partial class RunningGame
    {
        [Header("Player 데이터 관련")]
        public PlayerData[] playerDataArray = new PlayerData[3];
        public PlayerData CurrentPlayerData => playerDataArray[0];
        
        [Serializable]
        public class PlayerData
        {
            public int rank;
            public ReactiveProperty<int> score = new(0);
            public string name;
            [SerializeField] private MinMaxValue<float> scoreRandomIncreaseTimer = new(0, 0, 1, false, true);
            [SerializeField] private MinMax<int> scoreRandomIncrease = new(0,0);

            public ReactiveProperty<Color> mainColor = new (Color.white);

            public void RandomIncreaseScore(float deltaTime)
            {
                scoreRandomIncreaseTimer.Current += deltaTime;
                
                if (scoreRandomIncreaseTimer.IsMax)
                {
                    scoreRandomIncreaseTimer.Current -= scoreRandomIncreaseTimer.Max;
                    score.Value += scoreRandomIncrease.Random();
                }
            }
        }

        public void UpdatePlayerData()
        {
            for (var i = 0; i < playerDataArray.Length; i++)
            {
                var data = playerDataArray[i];
                data.RandomIncreaseScore(Time.deltaTime * gameSpeed.Value);
            }
        }
    }
    
    public partial class RunningGame
    {
        public override void GamePlay()
        {
            base.GamePlay();
            lobbyCanvas.gameObject.SetActive(false);
            lobbyObject.gameObject.SetActive(false);
            
            inGameCanvas.gameObject.SetActive(true);
            inGameObject.gameObject.SetActive(true);
            
            if(rankQuest) rankQuest.Play();
            
            foreach (ObjectSpawner spawner in obstacleSpawnerList)
                spawner.Play();
        }

        public override void GameStop()
        {
            base.GameStop();
            foreach (ObjectSpawner spawner in obstacleSpawnerList)
                spawner.Pause();
        }

        public override void GameClear()
        {
            base.GameClear();
            QuestManager.Instance.OnValueChange(QuestType.GameRank, CurrentPlayerData.rank);

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
            exitButton.onClick.AddListener(phone.applicationControl.OnHome);
            lobbyExitButton.onClick.AddListener(phone.applicationControl.OnHome);
        }

        public override void AppPlay(PhoneControl phone)
        {
            base.AppPlay(phone);
            runningGameObjectRoot.SetActive(true);
            runningGameCanvasRoot.gameObject.SetActive(true);

            rankQuest = QuestDataList.Instance.InstantiateEvent(rankEventID);
            
            // 게임 클리어 할 시
            GameManager.Instance.isGameClear.Subscribe(value =>
            {
                if(value)
                    isGamePlay.Value = false;
            });
        }

        public override void AppResume(PhoneControl phone)
        {
            base.AppResume(phone);
            runningGameObjectRoot.SetActive(true);
            runningGameCanvasRoot.gameObject.SetActive(true);
            InputManager.running.input.Enable();
            if(isGameStart && !settingCanvas.gameObject.activeSelf) GamePlay();
        }

        public override void AppPause(PhoneControl phone)
        {
            base.AppPause(phone);
            GameStop();
            runningGameObjectRoot.SetActive(false);
            runningGameCanvasRoot.gameObject.SetActive(false);
            InputManager.running.input.Disable();
        }

        public override void AppExit(PhoneControl phone)
        {
            base.AppExit(phone);
            runningGameObjectRoot.SetActive(false);
            runningGameCanvasRoot.gameObject.SetActive(false);
            InputManager.running.input.Disable();
        }
    }
}

