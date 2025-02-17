using GamePlay.Phone;
using Quest.UI;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GamePlay
{
    public partial class GameManager : Singleton<GameManager>
    {
        public ReactiveProperty<bool> isGameStart;
        public ReactiveProperty<bool> isGameClear;
        public MinMaxValue<float> playTimer = new(0, 0, 60 * 10);
        
        [Tooltip("방해 이벤트를 초기화(시작)했는지")] public bool isInitQuest = false;

        [Header("게임 결과 관련")] 
        public QuestResult questResultPrefab;
        
        public void Awake()
        {
            if (!SceneUtil.TryGetPhoneScene(out var scene))
            {
                void AddApp(Scene s)
                {
                    foreach (GameObject rootGameObject in s.GetRootGameObjects())
                    {
                        var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                        if(app != null) PhoneUtil.currentPhone.applicationControl.AddApp(app);
                    }
                }
                
                SceneUtil.AsyncAddPhone(phoneScene =>
                {
                    SceneUtil.AsyncAddChatting(AddApp);
                    SceneUtil.AsyncAddBank(AddApp);
                    SceneUtil.AsyncAddRunningGame(AddApp);
                });
            }

            isGameClear.Subscribe(value =>
            {
                if (value)
                {
                    Instantiate(questResultPrefab);
                }
            });
        }

        public void Start()
        {
            isGameStart.Value = true;
        }

        public void Update()
        {
            if (isGameStart.Value && !isGameClear.Value)
            {
                playTimer.Current += Time.deltaTime;
                if (playTimer.IsMax)
                {
                    GameClear();
                }
            }
        }
    }

    public partial class GameManager
    {
        public void GameClear()
        {
            isGameClear.Value = true;
        }
    }
}

