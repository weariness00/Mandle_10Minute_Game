using GamePlay.MiniGame.RunningGame.UI;
using GamePlay.Phone;
using System;
using Manager;
using Quest;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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

        [Header("Setting 관련")] 
        public Canvas settingCanvas;
        public Button continueButton;
        public Button exitButton;
        
        [Header("Lobby 관련")] 
        public Canvas lobbyCanvas;
        public GameObject lobbyObject;
        public Button lobbyExitButton;

        [FormerlySerializedAs("menuCanvas")] [Header("Menu 관련")] 
        public MatchingCanvas matchingCanvas;
        public GameObject matchingObject;
        
        [Header("In Game 관련")]
        public InGameCanvas inGameCanvas;
        public GameObject inGameObject;
        
        public List<ObjectSpawner> obstacleSpawnerList;

        [Header("기타 사항")] 
        [Tooltip("게임 클리어시 발동할 방해 이벤트")]public int bankQuestID;

        public override void Awake()
        {
            base.Awake();
            InputManager.running.input.Enable();

            settingCanvas.gameObject.SetActive(false);
            
            lobbyCanvas.gameObject.SetActive(true);
            lobbyObject.gameObject.SetActive(true);
            
            matchingCanvas.mainCanvas.gameObject.SetActive(false);
            matchingObject.SetActive(false);
            
            inGameCanvas.mainCanvas.gameObject.SetActive(false);
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
            
            matchingCanvas.gameStartButton.onClick.AddListener(() =>
            {
                matchingCanvas.mainCanvas.gameObject.SetActive(false);
                matchingObject.SetActive(false);
                
                GamePlay();
            });
            
            // 인게임 게임 시작 눌렀을때 카운트 다운 끝나고 동작
            inGameCanvas.onGameStart += () =>
            {
                isGamePlay.Value = true;
                foreach (ObjectSpawner spawner in obstacleSpawnerList)
                    spawner.Play();
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

            player.life.Subscribe(value =>
            {
                if (value <= 0)
                    GameOver();
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
            if (isOnTutorial)
            {
                base.GamePlay();

                lobbyCanvas.gameObject.SetActive(false);
                lobbyObject.gameObject.SetActive(false);
            
                inGameCanvas.mainCanvas.gameObject.SetActive(true);
                inGameObject.gameObject.SetActive(true);

                isGamePlay.Value = false;
                inGameCanvas.GameContinueCountDown();
            }
            else
            {
                // isOnTutorial 이 base에 변경된다.
                base.GamePlay();
            }
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
            QuestManager.Instance.OnValueChange(QuestType.MiniGameRank, CurrentPlayerData.rank);

            QuestManager.Instance.isQuestStart = false;
            var quest = QuestDataList.Instance.InstantiateEvent(bankQuestID);
            QuestManager.Instance.AddQuestQueue(quest);

            var home = Phone.applicationControl.GetHomeApp();
            if (home)
            {
                var appButton = home.GetAppButton(this);
                if (appButton)
                {
                    appButton.button.interactable = false;
                }
            }
        }

        public override void GameOver()
        {
            base.GameOver();
            QuestManager.Instance.OnValueChange(QuestType.MiniGameRank, CurrentPlayerData.rank);

            if(Phone)
                GameManager.Instance.isGameClear.Value = true;
            else
            {
                SceneUtil.LoadRunningGame();
            }
        }
    }

    public partial class RunningGame
    {
        public override void SetActiveBackground(bool value)
        {
            base.SetActiveBackground(value);
            runningGameObjectRoot.SetActive(value);
            runningGameCanvasRoot.gameObject.SetActive(value);
        }

        public override void AppInstall(PhoneControl phone)
        {
            base.AppInstall(phone);
            SetActiveBackground(false);
            
            // 나가기 누르면 앱으로 이동 ( 게임은 종료되지 않음 )
            exitButton.onClick.AddListener(phone.applicationControl.OnHome);
            lobbyExitButton.onClick.AddListener(phone.applicationControl.OnHome);
        }

        public override void AppPlay(PhoneControl phone)
        {
            base.AppPlay(phone);
            SetActiveBackground(true);
            
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
            SetActiveBackground(true);
            
            InputManager.running.input.Enable();
            settingCanvas.gameObject.SetActive(true);
        }

        public override void AppPause(PhoneControl phone)
        {
            base.AppPause(phone);
            GameStop();
            SetActiveBackground(false);

            InputManager.running.input.Disable();
        }

        public override void AppExit(PhoneControl phone)
        {
            base.AppExit(phone);
            SetActiveBackground(false);

            InputManager.running.input.Disable();
        }
    }
}

