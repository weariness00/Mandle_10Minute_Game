using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame
{
    public class ResultCanvas : MonoBehaviour
    {
        public Canvas mainCanvas;
        
        public ResultUIBlock resultBlockPrefab;
        public Transform resultGroupTransform;

        [Space]
        public Button okButton;

        [Space] 
        public RunningGame runningGame;

        public void InstantiateResult()
        {
            // 기존에 있던거 제거
            for (int i = 0; i < resultGroupTransform.childCount; i++)
                Destroy(resultGroupTransform.GetChild(i).gameObject);

            foreach (RunningGame.PlayerData data in runningGame.playerDataArray)
            {
                var block = Instantiate(resultBlockPrefab, resultGroupTransform);
                PhoneUtil.SetLayer(block);
                block.resultText.text = $"{data.rank}등 : {data.name}";
            }
        }
    }
}

