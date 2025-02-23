using DG.Tweening;
using KoreanTyper;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Util;

namespace GamePlay.Narration
{
    public class GamePlayerNarration : MonoBehaviour
    {
        public GameObject narrationObject; // 나레이션 오브젝트
        public CanvasGroup canvasGroup; // 알파 값 사용용도
        public TMP_Text narrationText; // 나레이션 텍스트
        public MinMaxValue<float> narrationReadTimer = new(0,0,1); // 나레이션 읽는 속도

        [SerializeField] private NarrationData currentData;
        
        public void Awake()
        {
            narrationReadTimer.SetMin();
            gameObject.SetActive(false);
        }

        public void Update()
        {
            if (!narrationReadTimer.IsMax)
            {
                narrationReadTimer.Current += Time.deltaTime;
                narrationText.text = currentData.text.Typing(narrationReadTimer.NormalizeToRange());
                if (narrationReadTimer.IsMax)
                {
                    DOVirtual.DelayedCall(currentData.stayDuration, () =>
                    {
                        canvasGroup.DOFade(0,4f).OnComplete(() => narrationObject.SetActive(true));
                    });
                }
            }
        }

        public void StartNarration(NarrationData data)
        {
            currentData = data;
            narrationReadTimer.SetMin();
            canvasGroup.alpha = 1;
            narrationObject.SetActive(true);
        }
    }
}

