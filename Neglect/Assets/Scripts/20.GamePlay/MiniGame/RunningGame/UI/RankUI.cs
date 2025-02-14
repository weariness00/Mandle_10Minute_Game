using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public class RankUI : MonoBehaviour
    {
        public RunningGame runningGame;
        public RankUIBlock dataBlockPrefab;

        public List<RankUIBlock> blockList;

        private List<Coroutine> blockPositionUpdateCoroutineList = new();
        
        public void Start()
        {
            blockList = new();
            UniqueRandom colorRandom = new(0.01f, 0.99f, 3, 2);
            foreach (var playerData in runningGame.playerDataArray)
            {
                var block = MakeUIBlock(playerData);
                block.rankIcon.color = Color.Lerp(Color.black, Color.white, colorRandom.RandomFloat());
            }
        }

        private RankUIBlock MakeUIBlock(RunningGame.PlayerData playerData)
        {
            var block = Instantiate(dataBlockPrefab, transform);
            block.data = playerData;
            block.nameText.text = playerData.name;
            blockList.Add(block);
            playerData.score.Subscribe(value =>
            {
                block.scoreText.text = $"{value}pt";
                UpdateRank();
            });

            return block;
        }

        private void UpdateRank()
        {
            var rankList = new List<RankUIBlock>(blockList);
            rankList.Sort((a,b) =>  b.data.score.Value.CompareTo(a.data.score.Value));
            
            foreach (Coroutine coroutine in blockPositionUpdateCoroutineList)
                StopCoroutine(coroutine);
            
            for (var i = 0; i < rankList.Count; i++)
            {
                var block = rankList[i];
                block.rankText.text = $"{i + 1}";

                var destPos = new Vector2(0, -25 + (-block.rectTransform.sizeDelta.y * i));
                blockPositionUpdateCoroutineList.Add(StartCoroutine(UpdateRankPositionEnumerator(block, destPos)));
            }
        }

        private IEnumerator UpdateRankPositionEnumerator(RankUIBlock block, Vector2 destPos)
        {
            MinMaxValue<float> timer = new(0, 0, 0.5f);
            var originPos = block.rectTransform.anchoredPosition;
            while (!timer.IsMax)
            {
                timer.Current += Time.deltaTime;
                block.rectTransform.anchoredPosition = Vector2.Lerp(originPos, destPos, timer.Current / timer.Max);
                yield return null;
            }
        }
    }
}

