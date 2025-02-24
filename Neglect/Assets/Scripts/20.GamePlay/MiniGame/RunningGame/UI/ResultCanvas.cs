using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame
{
    public class ResultCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;

        public GameObject timeOverObject; // 타임 오버 관련 오브젝트
        public GameObject resultObject; // 결과 관련 오브젝트
        
        public ResultUIBlock resultBlockPrefab;
        public Transform resultGroupTransform;

        [Space]
        public Button okButton;

        [Space] 
        public RunningGame runningGame;

        private IDisposable show;
        
        public void Awake()
        {
            resultObject.SetActive(false);
        }

        public void OnEnable()
        {
            show?.Dispose();
            timeOverObject.SetActive(true);
            resultObject.SetActive(false);
            show = Observable.Interval(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
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

