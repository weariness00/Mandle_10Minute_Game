using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace GamePlay.Event
{
    public class BankReadMemo : MonoBehaviour
    {
        // Start is called before the first frame update

        public CanvasGroup canvasGroup;
        public RectTransform memoRectTransform;
        public TextMeshProUGUI MemoText;

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


        public void TextSetting(string Name, string Account , int Amount)
        {
            MemoText.text = "";
            MemoText.text += Name + "\n";
            MemoText.text += "Account : " + Account + "\n";
            MemoText.text += "Amount : " + Amount.ToString() + "\n";
        }

        public void TextSetting(string Name, string Account, int Amount, String Password)
        {
            MemoText.text = "Password..." +Password +"\n";
            MemoText.text += Name + "\n";
            MemoText.text += "Account : " + Account + "\n";
            MemoText.text += "Amount : " + Amount.ToString() + "\n";
        }

    }
}