using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame
{
    public class ResultCanvas : MonoBehaviour
    {
        public RunningGame runningGame;
        
        [Space] 
        public Canvas mainCanvas;

        public GameObject timeOverObject; // 타임 오버 관련 오브젝트
        
        [Header("플레이 결과 관련")]
        public GameObject resultObject; // 결과 관련 오브젝트
        public ResultUIBlock resultBlockPrefab;
        public Transform resultGroupTransform;
        public Button resultOkButton;

        [Header("달성한 랭크 표시 관련")] 
        public Canvas rankResultCanvas;
        public Image rankIcon;
        public List<Sprite> rankIconList;
        [Tooltip("랭크 달성 축하 텍스트")] public TMP_Text rankText;
        public List<string> rankAchieveTextList;
        public Button rankOkButton;
        
        private IDisposable show;
        
        public void Awake()
        {
            resultObject.SetActive(false);
            rankResultCanvas.gameObject.SetActive(false);
            
            resultOkButton.onClick.AddListener(() =>
            {
                resultObject.SetActive(false);
                rankResultCanvas.gameObject.SetActive(true);

                var index = runningGame.CurrentPlayerData.rank - 1;
                rankIcon.sprite = rankIconList[index];
                rankText.text = rankAchieveTextList[index];
            });
            
            rankOkButton.onClick.AddListener(() =>
            {
                resultObject.SetActive(false);
                rankResultCanvas.gameObject.SetActive(false);
                mainCanvas.gameObject.SetActive(false);
            });
        }

        public void OnEnable()
        {
            show?.Dispose();
            timeOverObject.SetActive(true);
            resultObject.SetActive(false);
            show = Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
            {
                timeOverObject.SetActive(false);
                resultObject.SetActive(true);
            });
        }


        public void InstantiateResult()
        {
            // 기존에 있던거 제거
            for (int i = 0; i < resultGroupTransform.childCount; i++)
                Destroy(resultGroupTransform.GetChild(i).gameObject);

            List<RunningGame.PlayerData> list = new(runningGame.playerDataArray);
            list.Sort((a,b) => a.rank.CompareTo(b.rank));
            foreach (RunningGame.PlayerData data in list)
            {
                var block = Instantiate(resultBlockPrefab, resultGroupTransform);
                PhoneUtil.SetLayer(block);
                block.resultText.text = $"{data.rank}등 : {data.name}";
            }
        }
    }
}

