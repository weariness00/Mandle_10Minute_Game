using GamePlay.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessegeRoom : MonoBehaviour
{
    public string MessegeRoomName;
    public string MessegeRoomPhoneNumber;
    public List<MessgeContent> MessgeContents = new();
    [Header("복사할 메시지")]
    public ChatTextBox OtherMessages;
    public ChatTextBox MyMessages;

    public ChatTextBox PreTextBox;
    public List<ChatTextBox> OtherTextBoxs = new();
    public List<ChatTextBox> MyTextBoxs = new();
    public List<ChatTextBox> UsingTextBoxs = new();

    public RectTransform OtherTextBoxRect;
    public RectTransform MyTextBoxRect;



    public RectTransform ChatScrollBox;
    public void Awake()
    {
        Setting();
    }
    public void Setting()
    {
        float NextTextPosY = 0; //처음 스폰할 메시지 좌표Y.
        if (OtherTextBoxRect == null)
            OtherTextBoxRect = OtherMessages.GetComponent<RectTransform>();
        if(MyTextBoxRect == null)
            MyTextBoxRect = MyMessages.GetComponent<RectTransform>();


        for (int i = 0; i < MessgeContents.Count; i++)
        {
            if (MessgeContents[i].Type == 0)
            {
                if (OtherTextBoxs.Count == 0)
                {
                    PreTextBox = Instantiate(OtherMessages,Vector3.zero, OtherMessages.transform.rotation, ChatScrollBox.gameObject.transform);
                }
                if (OtherTextBoxs.Count > 0)
                {
                    PreTextBox = OtherTextBoxs[0];
                    OtherTextBoxs.RemoveAt(0);
                }
                UsingTextBoxs.Add(PreTextBox);
                PreTextBox.SetText(MessgeContents[i].Messege);
                Vector3 vect = new Vector3(50 + OtherTextBoxRect.rect.size.x / 2 * OtherTextBoxRect.localScale.x, -NextTextPosY - OtherTextBoxRect.rect.size.y * PreTextBox.transform.localScale.y, 0);
                PreTextBox.transform.localPosition = vect;
                NextTextPosY += OtherTextBoxRect.rect.size.y * OtherTextBoxRect.localScale.y + 50;

                ChatScrollBox.sizeDelta = new Vector2(0, NextTextPosY + MyTextBoxRect.rect.size.y / 2);
            }
            else if (MessgeContents[i].Type == 1)
            {
                if (MyTextBoxs.Count == 0)
                {
                    PreTextBox = Instantiate(MyMessages, Vector3.zero, OtherMessages.transform.rotation, ChatScrollBox.gameObject.transform);
                }
                if (MyTextBoxs.Count > 0)
                {
                    PreTextBox = MyTextBoxs[0];
                    MyTextBoxs.RemoveAt(0);
                }
                UsingTextBoxs.Add(PreTextBox);
                PreTextBox.SetText(MessgeContents[i].Messege);
                Vector3 vect = new Vector3(550 - MyTextBoxRect.rect.size.x / 2 * MyTextBoxRect.localScale.x, -NextTextPosY - MyTextBoxRect.rect.size.y * PreTextBox.transform.localScale.y, 0);
                PreTextBox.transform.localPosition = vect;

                NextTextPosY += MyTextBoxRect.rect.size.y * MyTextBoxRect.localScale.y + 50;

                ChatScrollBox.sizeDelta = new Vector2(0, NextTextPosY + MyTextBoxRect.rect.size.y / 2);
            }
        }
    }

}
[Serializable]
public class MessgeContent
{
    public string Messege; // 내용
    public int Type; // 0이면 상대 1 이면 자신
} 
