using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

namespace GamePlay.Event
{
    public class BankReadMemo : MonoBehaviour
    {
        // Start is called before the first frame update

        public TextMeshProUGUI MemoText;
        public GameObject MemoImage;
        public SpriteRenderer MemoRenderer;
        public bool isEnd;
        
        Sequence MemoSeq;

        public void Awake()
        {
            MemoRenderer = MemoImage.GetComponent<SpriteRenderer>();
            MemoSeq = DOTween.Sequence();
        }
        public void Start()
        {
            ShowAnimation();
        }
        public void HideAnimation()
        {
            if (!isEnd)
            {
                MemoSeq.Append(MemoRenderer.DOFade(0f, 1f));
                MemoSeq.Join(transform.DOLocalMoveX(5f, 1f).SetRelative(true));
                isEnd = true;
            }
        }
        public void ShowAnimation()
        {
            MemoSeq.Append(MemoRenderer.DOFade(0f, 1f).From());
            MemoSeq.Join(transform.DOLocalMoveX(transform.localPosition.x + 5f, 1f).From());
        }


        public void TextSetting(string Name, string Account , int Amount)
        {
            MemoText.text = "";
            MemoText.text += Name + "\n";
            MemoText.text += "Account : " + Account + "\n";
            MemoText.text += "Amount : " + Amount.ToString() + "\n";
        }

    }
}