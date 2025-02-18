using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace GamePlay.Event
{
    public class BankMemo : MonoBehaviour
    {
        // Start is called before the first frame update

        public CanvasGroup canvasGroup;
        public RectTransform memoRectTransform;
        public TMP_Text memoText;

        Sequence MemoSeq;

        public void Awake()
        {
            MemoSeq = DOTween.Sequence();
        }
        public void Start()
        {
            memoRectTransform.anchoredPosition = new(memoRectTransform.sizeDelta.x,0);
        }
        
        public void HideAnimation()
        {
            MemoSeq.Append(canvasGroup.DOFade(0f, 1f));
            MemoSeq.Join(memoRectTransform.DOAnchorPosX(memoRectTransform.sizeDelta.x, 1f).SetRelative(true)).OnComplete(()=>{
                gameObject.SetActive(false);
            });
             
        }
        public void ShowAnimation()
        {
            gameObject.SetActive(true);
            MemoSeq.Append(canvasGroup.DOFade(0f, 1f).From());
            MemoSeq.Join(memoRectTransform.DOAnchorPosX(-memoRectTransform.sizeDelta.x / 2, 1f));
        }
        
        
        public void TextSetting(string receiverName, string account, int transferMoney)
        {
            memoText.text = $"{receiverName}\n" + 
                            $"Account : {account}\n" + 
                            $"Transfer Money : {transferMoney}";
        }

    }
}