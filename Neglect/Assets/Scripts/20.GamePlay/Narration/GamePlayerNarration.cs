using DG.Tweening;
using KoreanTyper;
using Manager;
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

        public AudioClip typingSound;
        
        [SerializeField] private NarrationData currentData;

        private int currentTypingLength;
        private int prevTypingLength;
        private bool isStartNarration;
        
        public void Awake()
        {
            narrationReadTimer.SetMin();
            gameObject.SetActive(false);
            isStartNarration = false;
        }

        public void Update()
        {
            if(isStartNarration == false) return;
            if (!narrationReadTimer.IsMax)
            {
                narrationReadTimer.Current += Time.deltaTime;
                narrationText.text = currentData.text.Typing(narrationReadTimer.NormalizeToRange());
                
                currentTypingLength = narrationText.text.Length;
                // 타이핑 사운드 추가
                if (currentTypingLength != prevTypingLength)
                {
                    prevTypingLength = currentTypingLength;
                    var audioSource = SoundManager.Instance.GetAudioSource("Effect");
                    audioSource.PlayOneShot(typingSound);
                }
                
                if (narrationReadTimer.IsMax)
                {
                    isStartNarration = false;
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
            isStartNarration = true;
        }
    }
}

