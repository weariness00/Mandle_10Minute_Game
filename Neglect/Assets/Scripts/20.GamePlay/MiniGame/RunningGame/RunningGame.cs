using GamePlay.MiniGame.RunningGame.UI;
using GamePlay.Phone;
using System;
using Manager;
using Quest;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
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

        [Header("Menu 관련")] 
        public MatchingCanvas matchingCanvas;
        public GameObject matchingObject;
        
        [Header("In Game 관련")]
        public InGameCanvas inGameCanvas;
        public GameObject inGameObject;
        
        public List<ObjectSpawner> obstacleSpawnerList;

        [Header("Result 관련")] 
        public ResultCanvas resultCanvas;

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
            
            resultCanvas.mainCanvas.gameObject.SetActive(false);

            InputManager.running.ESC.performed += SettingOnOff;
            
            // 인게임 게임 시작 눌렀을때 카운트 다운 끝나고 동작
            inGameCanvas.onGameStart += () =>
            {
                lobbyCanvas.gameObject.SetActive(false);
                lobbyObject.SetActive(false);
                
                isGameStart.Value = true;
                isGamePlay.Value = true;
                foreach (ObjectSpawner spawner in obstacleSpawnerList)
                    spawner.Play();
            };
            
            // 매칭 시작 버튼 누르면
            matchingCanvas.gameStartButton.onClick.AddListener(() =>
            {
                matchingCanvas.matchLoadingObject.SetActive(true);
            });
            
            // 매칭 끝났을 경우
            matchingCanvas.onMatchedEvent.AddListener(() =>
            {
                matchingCanvas.mainCanvas.gameObject.SetActive(false);
                matchingObject.SetActive(false);
                
                QuestManager.Instance.Init();
                QuestManager.Instance.QuestStart();
                GamePlay();
            });
            
            // 등수 확인 후 매칭으로 이동
            resultCanvas.okButton.onClick.AddListener(() =>
            {
                settingCanvas.gameObject.SetActive(false);
                
                lobbyCanvas.gameObject.SetActive(false);
                lobbyObject.SetActive(false);
                
                matchingCanvas.mainCanvas.gameObject.SetActive(true);
                matchingObject.SetActive(true);
                
                inGameCanvas.mainCanvas.gameObject.SetActive(false);
                inGameObject.SetActive(false);
            
                resultCanvas.mainCanvas.gameObject.SetActive(false);
            });
            
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
        }

        public override void Update()
        {
            base.Update();

            if (isGameStart.Value)
            {
                UpdatePlayerData();
            }
        }

        private void SettingOnOff(InputAction.CallbackContext context)
        {
            settingCanvas.gameObject.SetActive(!settingCanvas.gameObject.activeSelf);

            if (isGameStart.Value)
            {
                if (isGamePlay.Value) GameStop();
                else  GamePlay();
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
            public MinMax<int> scoreRandomIncrease = new(0,0);

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
            InputManager.running.ESC.performed -= SettingOnOff;
            if (QuestManager.HasInstance)
            {
                QuestManager.Instance.OnValueChange(QuestType.MiniGameRank, CurrentPlayerData.rank);
                QuestManager.Instance.isQuestStart = false;
            }

            matchingCanvas.gameStartButton.onClick.RemoveAllListeners();
            matchingCanvas.gameStartButtonText.text = "게임 끝!";
            matchingCanvas.seasonText.text = "시즌 종료";
            matchingCanvas.warringText.text = "시즌이 종료되었습니다. 게임을 종료해주십시오.";
            matchingCanvas.gameStartButton.onClick.AddListener(() =>
            {
                if (Phone)
                {
                    GameManager.Instance.GameEnding();
                }
                else
                {
                    SceneUtil.LoadRunningGame();
                }
            });
            
            resultCanvas.mainCanvas.gameObject.SetActive(true);
            resultCanvas.InstantiateResult();
        }

        public override void GameOver()
        {
            base.GameOver();
            InputManager.running.ESC.performed -= SettingOnOff;
            if (QuestManager.HasInstance)
            {
                QuestManager.Instance.OnValueChange(QuestType.MiniGameRank, CurrentPlayerData.rank);
                QuestManager.Instance.isQuestStart = false;
            }

            if (GameManager.HasInstance)
            {
                GameManager.Instance.isGameStart.Value = false;
            }
            matchingCanvas.gameStartButton.onClick.RemoveAllListeners();
            matchingCanvas.gameStartButtonText.text = "게임 끝!";
            matchingCanvas.seasonText.text = "시즌 종료";
            matchingCanvas.warringText.text = "시즌이 종료되었습니다. 게임을 종료해주십시오.";
            matchingCanvas.gameStartButton.onClick.AddListener(() =>
            {
                if (Phone)
                {
                    GameManager.Instance.GameEnding();
                }
                else
                {
                    SceneUtil.LoadRunningGame();
                }
            });
            
            resultCanvas.mainCanvas.gameObject.SetActive(true);
            resultCanvas.InstantiateResult();
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
            
            GameManager.Instance.onLastEvent.AddListener(quest =>
            {
                quest.onIgnoreEvent.AddListener(callScreenQuest =>
                {
                    // 2번 무시
                    callScreenQuest.onIgnoreEvent.AddListener(q =>
                    {
                        foreach (PlayerData data in playerDataArray)
                        {
                            if (data == CurrentPlayerData) continue;
                            data.scoreRandomIncrease.Max = 50;
                            data.scoreRandomIncrease.Min = 50;
                        }
                    });
                });
            });
        }

        public override void AppPlay(PhoneControl phone)
        {
            base.AppPlay(phone);
            SetActiveBackground(true);
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

