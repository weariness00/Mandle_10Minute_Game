using System;
using UniRx;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame
{
    public class MiniGameBase : MonoBehaviour
    {
        [SerializeField] protected bool isGamePlay = true;
        [SerializeField] protected MinMaxValue<float> playTime = new(0,0, 60 * 10);

        public virtual void Awake()
        {
            playTime.SetMin();
        }

        public virtual void Update()
        {
            if (isGamePlay)
            {
                playTime.Current += Time.deltaTime;
            }
        }

        public virtual void GamePlay()
        {
            isGamePlay = true;
        }

        public virtual void GameStop()
        {
            isGamePlay = false;
        }
    }
}

