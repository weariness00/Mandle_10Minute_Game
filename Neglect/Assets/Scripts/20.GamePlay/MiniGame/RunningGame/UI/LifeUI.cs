using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.MiniGame.RunningGame
{
    public class LifeUI : MonoBehaviour
    {
        public RunningPlayer player;
        public Transform lifeGroupTransform;
        public Image lifePrefab;
        public List<Image> lifeImageList = new();
        
        public void Awake()
        {
            Debug.Assert(player != null, "Running Player가 존재하지 않습니다.");

            for (int i = 0; i < player.life.Value; i++)
            {
                var newLife = Instantiate(lifePrefab, lifeGroupTransform);
                lifeImageList.Add(newLife);
            }

            player.life.Subscribe(ChangeLife);
        }

        public void ChangeLife(int value)
        {
            if(value < 0) return;
            if (value > lifeImageList.Count)
            {
                var length = value - lifeImageList.Count;
                for (int i = 0; i < length; i++)
                {
                    var newLife = Instantiate(lifePrefab, lifeGroupTransform);
                    lifeImageList.Add(newLife);
                }
            }
            else
            {
                var length = lifeImageList.Count - value;
                if (length > 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        var lastIndex = lifeImageList.Count - 1;
                        var image = lifeImageList[lastIndex];
                        lifeImageList.RemoveAt(lastIndex);
                        Destroy(image.gameObject);
                    }
                }
            }
        }
    }
}

