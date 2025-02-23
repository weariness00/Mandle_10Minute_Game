using GamePlay.MiniGame;
using GamePlay.Phone;
using Manager;
using Quest;
using System.Collections;
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

        [Tooltip("나레이션 클래스")] public GamePlayerNarration narration;
        [Tooltip("포스트 프로세싱을 사용할 Global Volume")]public PostProcessingUtility realVolumeControl;
        [Tooltip("방해 이벤트를 초기화(시작)했는지")] public bool isInitQuest = false;

        [Header("사전에 사용할 이벤트 ID")] 
        public QuestBase gameClearQuest;
        public int batteryEventID;
        public int introPopUpID;
        
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
                    StartCoroutine(loadedHomeAppEnumerator());
                    
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

        public IEnumerator loadedHomeAppEnumerator()
        {
            while (ReferenceEquals(PhoneUtil.currentPhone, null) || ReferenceEquals(PhoneUtil.currentPhone.applicationControl.GetHomeApp(), null))
                yield return null;

            var phone = PhoneUtil.currentPhone;
            var home = phone.applicationControl.GetHomeApp();
            home.firstStartWindow.clickEntry.callback.AddListener(data =>
            {
                var quest = QuestDataList.Instance.InstantiateEvent(introPopUpID);
                QuestManager.Instance.AddQuestQueue(quest);
                
                // 친구의 팝업을 완료하면
                quest.onCompleteEvent.AddListener(q1 =>
                {
                    // 친구와의 대화를 완료 하면 
                    q1.onCompleteEvent.AddListener(q2 =>
                    {
                        // 나레이션 시작
                        narration.StartNarration(); 

                        // 미니 게임 버튼 활성화
                        var miniGame = phone.applicationControl.GetApp<MiniGameBase>();
                        var miniGameAppButton = home.GetAppButton(miniGame);
                        miniGameAppButton.button.interactable = true;
                    });
                });
            });
        }
    }

    public partial class GameManager
    {
        public void GameClear()
        {
            QuestManager.Instance.isQuestStart = false;
            QuestManager.Instance.AddQuestQueue(gameClearQuest);
            gameClearQuest.Play();
            QuestManager.Instance.OnValueChange(QuestType.GameClear, playTimer.Current);
            isGameClear.Value = true;
            
            Destroy(gameObject);
        }
    }
}

