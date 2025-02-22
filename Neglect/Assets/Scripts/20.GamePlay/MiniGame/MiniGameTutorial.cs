using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class MiniGameTutorial : MonoBehaviour
{
    public Button okButton;
    public Button leftButton;
    public Button rightButton;
    public TMP_Text pageText;
    
    [Space]
    [Tooltip("튜토리얼로 보여줄 이미지")]public Image tutorialImage;
    [Tooltip("튜토리얼에 보여줄 이미지 순서")]public List<Sprite> tutorialImageList;

    private MinMaxValue<int> pageIndex;
    public void Awake()
    {
        pageIndex = new(0,0,tutorialImageList.Count -1);
        PageUpdate();
        
        leftButton.onClick.AddListener(LeftPage);
        rightButton.onClick.AddListener(RightPage);
    }
    
    public void LeftPage()
    {
        pageIndex.Current--;

        if (pageIndex.IsMin)
            leftButton.interactable = false;
        if (!pageIndex.IsMax)
            rightButton.interactable = true;
        PageUpdate();
    }

    public void RightPage()
    {
        pageIndex.Current++;

        if (!pageIndex.IsMin)
            leftButton.interactable = true;
        if (pageIndex.IsMax)
            rightButton.interactable = false;
        PageUpdate();
    }

    private void PageUpdate()
    {
        tutorialImage.sprite =  tutorialImageList[pageIndex.Current];
        pageText.text = $"{pageIndex.Current + 1} / {pageIndex.Max + 1}";
    }
}

