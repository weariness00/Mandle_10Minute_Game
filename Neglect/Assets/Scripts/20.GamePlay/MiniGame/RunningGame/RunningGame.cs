using GamePlay.MiniGame.RunningGame.UI;
using GamePlay.Phone;
using System;
using Manager;
using Quest;
using Quest.Container;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
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
        
        [FormerlySerializedAs("inGameCanvas")] [Header("In Game 관련")]
        public InGame inGame;
        public GameObject inGameObject;

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
            
            inGame.mainCanvas.gameObject.SetActive(false);
            inGameObject.gameObject.SetActive(false);
            
            resultCanvas.mainCanvas.gameObject.SetActive(false);
            
            InputManager.running.ESC.performed += SettingOnOff;
            
            // 인게임 게임 시작 눌렀을때 카운트 다운 끝나고 동작
            inGame.onGameStart += () =>
            {
                lobbyCanvas.gameObject.SetActive(false);
                lobbyObject.SetActive(false);
                
                isGameStart.Value = true;
                isGamePlay.Value = true;
                
                // 플레이어 애니메이션 활성화
                player.animator.animator.speed = 1f;
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

                if (QuestManager.HasInstance)
                {
                    QuestManager.Instance.Init();
                    QuestManager.Instance.QuestStart();
                }
                GamePlay();
            });
            
            // 등수 확인 후 매칭으로 이동
            resultCanvas.resultOkButton.onClick.AddListener(() =>
            {
                settingCanvas.gameObject.SetActive(false);
                
                lobbyCanvas.gameObject.SetActive(false);
                lobbyObject.SetActive(false);
                
                matchingCanvas.mainCanvas.gameObject.SetActive(true);
                matchingObject.SetActive(true);
                
                inGame.mainCanvas.gameObject.SetActive(false);
                inGameObject.SetActive(false);
            });

            player.life.Subscribe(value =>
            {
                if (value <= 0)
                    GameOver();
            });
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();

            if (isGameStart.Value)
            {
                UpdatePlayerData();
            }
            
            if (runningGameObjectRoot.activeSelf && !ReferenceEquals(_phone, null) && _phone.viewType == PhoneViewType.Vertical)
            {
                _phone.PhoneViewRotate(PhoneViewType.Horizon);
            }
        }

        private void SettingOnOff(InputAction.CallbackContext context)
        {
            if (inGame.gameObject.activeSelf)
            {
                settingCanvas.gameObject.SetActive(true);
                GameStop();
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
            public MinMaxValue<float> scoreRandomIncreaseTimer = new(0, 0, 1, false, true);
            public MinMax<int> scoreRandomIncrease = new(0,0);
            [HideInInspector] public float increaseMultiple = 1f;

            public ReactiveProperty<Color> mainColor = new (Color.white);

            private float originIncreaseMultiple = 1f;
            private IDisposable _disposable;
            
            public void RandomIncreaseScore(float deltaTime)
            {
                scoreRandomIncreaseTimer.Current += deltaTime;
                
                if (scoreRandomIncreaseTimer.IsMax)
                {
                    scoreRandomIncreaseTimer.Current -= scoreRandomIncreaseTimer.Max;
                    score.Value += scoreRandomIncrease.Random();
                }
            }

            public void SetMultiple(float value)
            {
                _disposable?.Dispose();
                originIncreaseMultiple = value;
                increaseMultiple = value;
            }
            
            public void StartMultipleDuration(float value, float duration)
            {
                _disposable?.Dispose();
                increaseMultiple = originIncreaseMultiple;
                originIncreaseMultiple = increaseMultiple;
                increaseMultiple = value;
                _disposable = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ => { increaseMultiple = originIncreaseMultiple; });
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
            
                inGame.mainCanvas.gameObject.SetActive(true);
                inGameObject.gameObject.SetActive(true);

                isGamePlay.Value = false;
                inGame.GameContinueCountDown();
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
            inGame.StopCountDown();
            
            player.animator.animator.speed = 0;
        }

        public override void GameClear()
        {
            if(isGameClear.Value) return;
            
            base.GameClear();
            inGame.StopCountDown();
            InputManager.running.ESC.performed -= SettingOnOff;
            if (QuestManager.HasInstance)
            {
                QuestManager.Instance.OnValueChange(QuestType.MiniGameRank, CurrentPlayerData.rank);
                QuestManager.Instance.isQuestStart = false;
            }

            matchingCanvas.gameStartButton.onClick.RemoveAllListeners();
            matchingCanvas.gameStartButtonText.text = "게임 끝!";
            matchingCanvas.seasonText.text = "시즌 종료";
            matchingCanvas.warringText.text = "시즌이 종료되었습니다.";
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
            resultCanvas.resultOkButton.onClick.AddListener(() =>
            {
                if (GameManager.HasInstance)
                {
                    if(CurrentPlayerData.rank == 1)
                        GameManager.Instance.ending.GoodEnding();
                    else
                        GameManager.Instance.ending.BadEnding();
                }
            });
        }

        public override void GameOver()
        {
            base.GameOver();
            inGame.StopCountDown();
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
            matchingCanvas.warringText.text = "시즌이 종료되었습니다.";
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

            inGame.AppInstall();
            
            QuestManager.Instance.onEndQuestEvent.AddListener(quest =>
            {
                // 어떤 이벤트든 실패했을 경우
                if (quest.state == QuestState.Failed)
                {
                    for (var i = 1; i < playerDataArray.Length; i++)
                    {
                        playerDataArray[i].StartMultipleDuration(2f, 30f);
                    }
                }
            });
            GameManager.Instance.onLastEvent.AddListener(quest =>
            {
                quest.onIgnoreEvent.AddListener(callScreenQuest =>
                {
                    // 2번 무시
                    callScreenQuest.onIgnoreEvent.AddListener(q =>
                    {
                        for (var i = 1; i < playerDataArray.Length; i++)
                        {
                            playerDataArray[i].scoreRandomIncreaseTimer.Max *= 0.3f;
                            playerDataArray[i].SetMultiple(2f);
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
            if(inGameObject.activeSelf) settingCanvas.gameObject.SetActive(true);
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

#if UNITY_EDITOR

    [CustomEditor(typeof(RunningGame), true)]
    public class RunningGameEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as RunningGame;

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("1등으로 게임 클리어"))
                {
                    script.CurrentPlayerData.rank = 1;
                    script.GameClear();
                }
            }
            base.OnInspectorGUI();
        }
    }
#endif
}

