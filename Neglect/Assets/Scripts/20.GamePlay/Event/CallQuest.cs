using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallQuest : MonoBehaviour
{
    // Start is called before the first frame update


    public string CurrentName = "임시 엠피시";
    public Text ChatName;


    [Header("말풍선")]
    [Tooltip("상대방 말풍선")]
    public Text OtherText;
    public Image OtherChat;

    [Tooltip("내 말풍선")]
    public Text MyText;
    public Image MyChat;

    [Space]
    [Header("선택지 버튼")]
    public List<Button> SelectButtons;
    public List<Image> SelectImages;
    public List<Text> SelectTexts;

    [Space]
    [Header("게이지")]
    public float ChatGage;
    public Image GageBar;

    [Header("데이터베이스에서 가져올 정보")]
    [Space]
    public string Question;
    public string[] replyString;
    public int[] replygage;

    
    Sequence MainSequence;

    public void Awake()
    {
        MainSequence = DOTween.Sequence();

        ChatName.text = CurrentName;
        ChatStart();
    }
    public void ResetObject()
    {
        OtherChat.gameObject.SetActive(false);
        OtherText.gameObject.SetActive(false);
        MyChat.gameObject.SetActive(false);
        MyText.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            SelectTexts[i].gameObject.SetActive(false);
            SelectButtons[i].gameObject.SetActive(false);
        }
    }

    public void ChatStart()
    {
        ResetObject();

        // 데이터베이스 재설정 코드 넣을 것.

        Sequence UiSeq = DOTween.Sequence();
        UiSeq.AppendCallback(() =>
        {
            OtherChat.gameObject.SetActive(true);
        });
        UiSeq.Append(OtherChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(OtherChat.DOFade(0f, 0f)).Join(OtherChat.DOFade(1f, 0.5f));
        UiSeq.AppendCallback(() =>
        { OtherText.gameObject.SetActive(true);
            OtherText.text = Question;
        });
        // ~ 상대 말풍선 애니메이션


        UiSeq.AppendCallback(() =>
        { for (int i = 0; i < 3; i++) {
                SelectButtons[i].interactable = true;
                SelectButtons[i].gameObject.SetActive(true); } 
        });
        for (int i = 0; i < 3; i++)
        {
            SelectImages[i].DOFade(0f, 0f);
            UiSeq.Append(SelectButtons[i].gameObject.transform.DOLocalMoveY(SelectButtons[i].gameObject.transform.localPosition.y - 10f, 0.5f)
            .From()).Join(SelectImages[i].DOFade(1f, 0.5f));
        }
        UiSeq.AppendCallback(() =>
        {
            for (int i = 0; i < 3; i++)
            {
               SelectTexts[i].gameObject.SetActive(true);
                Color reset = SelectTexts[i].color;
                reset.a = 1f;
                SelectTexts[i].color = reset;
                SelectTexts[i].text = replyString[i];
            }
        });
        //~ 버튼 나오는 애니메이션
    }

    public void ChoiceBttons(int index) //버튼 클릭시
    {
        Sequence UiSeq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            SelectButtons[i].interactable = false;
            if (i != index)
                UiSeq.Join(SelectImages[i].DOFade(0f, 0.5f)).Join(SelectTexts[i].DOFade(0f, 0.5f));
        }
        UiSeq.Append(SelectImages[index].DOFade(0f, 0.5f)).Join(SelectTexts[index].DOFade(0f, 0.5f));
        // ~버튼 종료 애니메이션



        UiSeq.AppendCallback(() =>
        {
            MyChat.gameObject.SetActive(true); });
        UiSeq.Append(MyChat.gameObject.transform.DOLocalMoveY(10f, 0.5f).From().SetRelative(true)).Join(MyChat.DOFade(0f, 0f)).Join(MyChat.DOFade(1f, 0.5f));
        UiSeq.AppendCallback(() =>
        {
            MyText.gameObject.SetActive(true);
            MyText.text = replyString[index];
        });
        // ~ 내 채팅 나오는 애니메이션


        ChatGage = (ChatGage + replygage[index] > 100) ? 100 : ChatGage + replygage[index];
        ChatGage = (ChatGage < 0) ? 0 : ChatGage;

        UiSeq.Append(
        GageBar.DOFillAmount(ChatGage/100f , 1f));
        // 게이지 차는 애니메이션


        if (ChatGage < 100)
            UiSeq.AppendCallback(() => ChatStart()); // 반복
        else
        {
            // 이벤트 종료
            Debug.Log("이벤트 종료");
        }
    }
}

