using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Chatting
{
    public class ChatTextBox : MonoBehaviour
    {
        // Start is called before the first frame update
        public RectTransform boxTransform;
        public Image image;
        public TextMeshProUGUI Text;

        public float paddingLeft;
        public float paddingRight;
        public float paddingBottom;
        public float paddingTop;

        public float maxWidth;

        private RectTransform _rectTransform;

        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Text.enableWordWrapping = true; // 자동 줄바꿈 활성화
        }

        public void Update()
        {
            _rectTransform.sizeDelta = new(_rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y);
            
            float width = Text.preferredWidth;
            float height = Text.preferredHeight;

            if (width > maxWidth)
            {
                Text.rectTransform.sizeDelta = new Vector2(maxWidth, height);
                height = Text.preferredHeight; // 줄바꿈 후 높이 다시 계산
                width = maxWidth; // 고정된 최대 너비 적용
            }
            
            Text.rectTransform.sizeDelta = new Vector2(width, height);
            Text.rectTransform.anchoredPosition = new Vector3(paddingLeft, -paddingTop);
            image.rectTransform.sizeDelta = new Vector2(width + paddingLeft + paddingRight, height + paddingTop + paddingBottom);
        }

        public void SetText(string text)
        {
            Text.text = InsertNewlines(text,10);
        }
        
        public static string InsertNewlines(string input, int chunkSize = 10)
        {
            return Regex.Replace(input, $"(.{{{chunkSize}}})", "$1\n");
        }

    }
}