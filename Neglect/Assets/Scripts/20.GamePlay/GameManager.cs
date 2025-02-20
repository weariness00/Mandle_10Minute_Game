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

        public PostProcessingUtility realVolumeControl;
        
        public void Awake()
        {
            if (!SceneUtil.TryGetPhoneScene(out var s))
            {
                void AddApp(Scene scene)
                {
                    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
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
                    // 게임 클리어하면 결과씬 로드
                    SceneUtil.AsyncAddGameResult(scene =>
                    {
                        foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                        {
                            var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                            if (app != null)
                            {
                                var phone = PhoneUtil.currentPhone;
                                phone.applicationControl.AddApp(app);
                                phone.applicationControl.OpenApp(app);
                            }
                        }
                    });
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

