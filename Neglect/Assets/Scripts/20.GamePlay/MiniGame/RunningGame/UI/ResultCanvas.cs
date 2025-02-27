using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;

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

        public MMF_Player timeOverMMF; //타임 오버 MMF
        public MMF_Player showResultTextMMF; //처음 결과값 나오는 MMF;
        public MMF_Player[] showRankMMF = new MMF_Player[3]; //금 은 동 트로피 나오는 MMF;
        public MMF_Player showOkButtonAndTextMMF; //텍스트 나오는 MMF;
        public List<ResultUIBlock> resultUIBlocks;

        public void Awake()
        {
            resultObject.SetActive(false);
            resultOkButton.gameObject.SetActive(false);
            rankResultCanvas.gameObject.SetActive(false);
            
            // 게임 전체 결과 화면
            resultOkButton.onClick.AddListener(() =>
            {
                resultObject.SetActive(false);
                rankResultCanvas.gameObject.SetActive(true);

                var index = runningGame.CurrentPlayerData.rank - 1;
                rankIcon.sprite = rankIconList[index];
                rankText.text = rankAchieveTextList[index];
                showRankMMF[index].PlayFeedbacks();

                rankOkButton.gameObject.SetActive(false);
                rankText.gameObject.SetActive(false);
            });
            
            // 매칭에서 순위 결과 화면
            rankOkButton.onClick.AddListener(() =>
            {
                resultObject.SetActive(false);
                rankResultCanvas.gameObject.SetActive(false);
                mainCanvas.gameObject.SetActive(false);
            });
            
            // 타임 오버 연출 완료되면
            timeOverMMF.Events.OnComplete.AddListener(() =>
            {
                timeOverObject.SetActive(false);
                resultObject.SetActive(true);

                showResultTextMMF.PlayFeedbacks(); // mmf 애니메이션
            });
            
            // 모든 플레이어 순위 보여주는 연출 완료되면
            showResultTextMMF.Events.OnComplete.AddListener(() =>
            {
                resultOkButton.gameObject.SetActive(true);
                resultOkButton.image.DOFade(0, 0);
                resultOkButton.image.DOFade(1f, 1f);
            });
        }
        public void RankOkButtonAndTextShow() // 트로피 MMF 끝나고 실행되겠끔 설정해둠
        {
            rankOkButton.gameObject.SetActive(true);
            rankText.gameObject.SetActive(true);
            showOkButtonAndTextMMF.PlayFeedbacks();
        }
        public void OnEnable()
        {
            show?.Dispose();
            timeOverObject.SetActive(true);
            timeOverMMF.PlayFeedbacks();
            resultObject.SetActive(false);
        }

        public void InstantiateResult()
        {
            // 기존에 있던거 제거
            for (int i = 0; i < resultGroupTransform.childCount; i++)
                Destroy(resultGroupTransform.GetChild(i).gameObject);

            List<RunningGame.PlayerData> list = new(runningGame.playerDataArray);
            list.Sort((a,b) => a.rank.CompareTo(b.rank));
            
            resultUIBlocks.Clear();
            foreach (RunningGame.PlayerData data in list)
            {
                var block = Instantiate(resultBlockPrefab, resultGroupTransform);
                PhoneUtil.SetLayer(block);
                block.resultText.text = $"{data.rank}등 : {data.name}";
                resultUIBlocks.Add(block);
            }
            InitShowResultTextMMF();
        }

        public void InitShowResultTextMMF()
        {
            int i = 0;
            foreach (MMF_Feedback Fed in showResultTextMMF.FeedbacksList)
            {
                if(Fed.Label == "TextSAS")
                {
                    if (Fed is MMF_SquashAndStretch sASFeedback)
                    {
                        sASFeedback.SquashAndStretchTarget = resultUIBlocks[i++].transform;
                    }
                }
            }
        }
    }
}

