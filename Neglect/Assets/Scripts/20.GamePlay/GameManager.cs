using GamePlay.MiniGame;
using GamePlay.Narration;
using GamePlay.Phone;
using Quest;
using Quest.Container;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace GamePlay
{
    public partial class GameManager : Singleton<GameManager>
    {
        public ReactiveProperty<bool> isGameStart;
        public ReactiveProperty<bool> isGameClear;
        public MinMaxValue<float> playTimer = new(0, 0, 60 * 10);
        public float lastEventTime = 60f * 8f;

        [Tooltip("게임 엔딩 관리")] public GameEnding ending;
        [Tooltip("나레이션 클래스")] public GamePlayerNarration narration;
        [Tooltip("포스트 프로세싱을 사용할 Global Volume")]public PostProcessingUtility realVolumeControl;
        [Tooltip("방해 이벤트를 초기화(시작)했는지")] public bool isInitQuest = false;

        [Space] 
        [Tooltip("월드 전체 Canvas")]public Canvas worldCanvas;
        [Tooltip("단순 비어있는 ui image")] public Image emptyUIPrefab;
        
        [Header("사전에 사용할 이벤트 ID")] 
        public QuestBase gameClearQuest;
        public int batteryEventID;
        public int introPopUpID;
        public int lastEventID;

        [HideInInspector] public UnityEvent<QuestBase> onLastEvent;
        
        public void Awake()
        {
            QuestManager.Instance.Init();
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
                    StartCoroutine(LoadedHomeAppEnumerator());
                    
                    SceneUtil.AsyncAddDummy(AddApp);
                    SceneUtil.AsyncAddTutorial(AddApp);
                    SceneUtil.AsyncAddChatting(AddApp);
                    SceneUtil.AsyncAddBank(AddApp);
                    SceneUtil.AsyncAddRunningGame(AddApp);
                });
            }
        }

        public void Update()
        {
            if (isGameStart.Value && !isGameClear.Value)
            {
                playTimer.Current += Time.deltaTime;
                if (lastEventID != -1 && playTimer.Current >= lastEventTime)
                {
                    // 마지막 이벤트가 동작되면 이벤트 랜덤 생성 정지
                    QuestManager.Instance.isQuestStart = false;
                    
                    var lastQuest = QuestDataList.Instance.InstantiateEvent(lastEventID);
                    onLastEvent?.Invoke(lastQuest);
                    QuestManager.Instance.AddQuestQueue(lastQuest);
                    lastEventID = -1;
                }
                
                if (playTimer.IsMax)
                {
                    GameClear();
                }
            }
        }

        public IEnumerator LoadedHomeAppEnumerator()
        {
            while (ReferenceEquals(PhoneUtil.currentPhone, null) || ReferenceEquals(PhoneUtil.currentPhone.applicationControl.GetHomeApp(), null))
                yield return null;

            var ui = PhoneUtil.InstantiateUI(emptyUIPrefab);
            ui.color = new Color(0, 0, 0, 0.7f);
            var phone = PhoneUtil.currentPhone;
            var home = phone.applicationControl.GetHomeApp();
            home.firstStartWindow.clickEntry.callback.AddListener(data =>
            {
                var quest = QuestDataList.Instance.InstantiateEvent(introPopUpID);
                QuestManager.Instance.AddQuestQueue(quest);
                if (quest is Quest_ChattingPopUp chattingPopUp)
                {
                    chattingPopUp.popUp.destroyTimer.Max = 999999999;
                    chattingPopUp.popUp.destroyMoveDistance = Vector2.positiveInfinity;
                }
                
                quest.onCompleteEvent.AddListener(q => Destroy(ui.gameObject));
                quest.onIgnoreEvent.AddListener(q => Destroy(ui.gameObject));
                
                // 친구의 팝업을 완료하면
                quest.onCompleteEvent.AddListener(q1 =>
                {
                    // 친구와의 대화를 완료 하면 
                    q1.onCompleteEvent.AddListener(q2 =>
                    {
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
            if(isGameClear.Value) return;
            isGameClear.Value = true;
            
            QuestManager.Instance.isQuestStart = false;
            QuestManager.Instance.AddAndPlay(gameClearQuest);
            QuestManager.Instance.OnValueChange(QuestType.GameClear, playTimer.Current);
            QuestManager.Instance.AllQuestFailed();
        }

        public void GameEnding()
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
            Destroy(gameObject);
        }
    }
}

