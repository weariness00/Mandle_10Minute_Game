using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public partial class InGame : MonoBehaviour
    {
        public RunningGame runningGame;
        public GameObject mainObject;
        public Canvas mainCanvas;

        [Header("Game Play 관련")] 
        [Tooltip("몇초마다 게임에 변화를 줄 것인지")] public MinMaxValue<float> updateGameChangeTimer = new(0,0,150, false, true);
        [HideInInspector] public UnityEvent onGameChangeEvent = new();
        public List<ObjectSpawner> objectSpawnerList;
        public ObjectSpawner eventIgnoreSpawner;
        [HideInInspector] public ObjectSpawner currentSpawner;
        [HideInInspector] public int spawnerIndex = 0;
        
        [Header("Game Continue 관련")] 
        public Canvas continueCanvas;
        public TMP_Text countDownText;
        public List<Color> countDownTextColor = new List<Color>();
        [HideInInspector] public Action onGameStart;

        public void Awake()
        {
            continueCanvas.gameObject.SetActive(false);
         
            currentSpawner = objectSpawnerList[spawnerIndex];
            objectSpawnerList.Add(eventIgnoreSpawner);
            foreach (ObjectSpawner spawner in objectSpawnerList)
            {
                spawner.SpawnSuccessAction.AddListener(obj =>
                {
                    PhoneUtil.SetLayer(obj);
                    SceneManager.MoveGameObjectToScene(obj, SceneUtil.GetRunningGameScene());
                    obj.GetComponent<RunningObstacle>().runningGame = runningGame;
                    obj.transform.SetParent(mainObject.transform);
                });
            }
            objectSpawnerList.Remove(eventIgnoreSpawner);
            
            onGameChangeEvent.AddListener(() =>
            {
                if (spawnerIndex < objectSpawnerList.Count - 1)
                {
                    currentSpawner.Pause();
                    currentSpawner = objectSpawnerList[++spawnerIndex];
                    if(runningGame.isGamePlay.Value)
                        currentSpawner.Play(1f);
                }
            });

            runningGame.isGamePlay.Subscribe(value =>
            {
                if(value) currentSpawner.Play();
                else currentSpawner.Stop();
            });
            
            runningGame.gameSpeed.Subscribe(value =>
            {
                foreach (ObjectSpawner spawner in objectSpawnerList)
                    spawner.timeScale = value;
            });
        }

        public void Update()
        {
            if (runningGame.isGamePlay.Value)
            {
                updateGameChangeTimer.Current += Time.deltaTime;
                if (updateGameChangeTimer.IsMax)
                {
                    updateGameChangeTimer.Current -= updateGameChangeTimer.Max;
                    onGameChangeEvent?.Invoke();
                }
            }
        }

        // 게임 시작 카운트 다운
        public void GameContinueCountDown(float count = 3f)
        {
            continueCanvas.gameObject.SetActive(true);
            StartCoroutine(GameContinueEnumerator(count));
        }

        public void StopCountDown()
        {
            StopAllCoroutines();
        }
        
        private IEnumerator GameContinueEnumerator(float count = 3f)
        {
            int startCount = (int)count + 1;
            while (true)
            {
                count -= Time.deltaTime;
                int value = ((int)Mathf.Round(count));
                if (value <= -1) break;
                countDownText.text = value.ToString();
                countDownText.color = countDownTextColor[startCount - (value + 1)];
                if (value <= 0)
                    countDownText.text = "GO!";
                yield return null;
            }
            
            continueCanvas.gameObject.SetActive(false);
            onGameStart?.Invoke();
        }

        private IEnumerator IgnoreSpawnerEnumerator()
        {
            while (!runningGame.isGamePlay.Value)
                yield return null;
            if (currentSpawner != eventIgnoreSpawner)
            {
                currentSpawner.Stop();
                currentSpawner = eventIgnoreSpawner;
                if(runningGame.isGamePlay.Value)
                    currentSpawner.Play(1f);
            }
            
            yield return new WaitForSeconds(20f);
            if (currentSpawner == eventIgnoreSpawner)
            {
                currentSpawner.Stop();
                currentSpawner = objectSpawnerList[spawnerIndex];
                if(runningGame.isGamePlay.Value)
                    currentSpawner.Play(1f);
            }

            ignoreSpawnerCoroutine = null;
        }
    }

    // Running Game 스크립트에서 사용할 App 관련 함수
    public partial class InGame
    {
        private Coroutine ignoreSpawnerCoroutine;
        public void AppInstall()
        {
            QuestManager.Instance.onEndQuestEvent.AddListener(quest =>
            {
                if (quest.state == QuestState.Failed)
                {
                    if(ignoreSpawnerCoroutine != null) StopCoroutine(ignoreSpawnerCoroutine);
                    ignoreSpawnerCoroutine = StartCoroutine(IgnoreSpawnerEnumerator());
                }
                else if (quest.state == QuestState.Completed && currentSpawner == eventIgnoreSpawner)
                {
                    currentSpawner.Stop();
                    currentSpawner = objectSpawnerList[spawnerIndex];
                    currentSpawner.Play(1f);
                }
            });
        }
    }
}

