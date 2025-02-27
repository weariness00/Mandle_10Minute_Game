using DG.Tweening;
using System;
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
        private List<Tween> blockPositionUpdateTweenList = new();
        
        public void Start()
        {
            blockList = new();
            for (var i = 0; i < runningGame.playerDataArray.Length; i++)
            {
                var playerData = runningGame.playerDataArray[i];
                var block = MakeUIBlock(playerData);
                var destPos = new Vector2(0, -25 + (-block.rectTransform.sizeDelta.y * i));
                blockPositionUpdateCoroutineList.Add(StartCoroutine(UpdateRankPositionEnumerator(block, destPos)));
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
                if (runningGame.isGamePlay.Value)
                    block.scoreText.text = $"{value}pt";
                UpdateRank();
            });

            return block;
        }

        private void UpdateRank()
        {
            var rankList = new List<RankUIBlock>(blockList);
            rankList.Sort((a,b) =>  b.data.score.Value.CompareTo(a.data.score.Value));
            
            foreach (Tween tween in blockPositionUpdateTweenList)
                tween?.Kill();
            blockPositionUpdateTweenList.Clear();
            
            for (var i = 0; i < rankList.Count; i++)
            {
                var block = rankList[i];
                block.data.rank = i + 1;

                if (runningGame.isGamePlay.Value)
                {
                    block.rankText.text = $"{i + 1}";
                    var destPos = new Vector2(0, -25 + (-block.rectTransform.sizeDelta.y * i));
                    if (Math.Abs(block.rectTransform.anchoredPosition.y - destPos.y) > 0.1f)
                    {
                        var tween = block.rectTransform.DOAnchorPos(destPos, 0.5f);
                        blockPositionUpdateTweenList.Add(tween);
                    }
                }
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

