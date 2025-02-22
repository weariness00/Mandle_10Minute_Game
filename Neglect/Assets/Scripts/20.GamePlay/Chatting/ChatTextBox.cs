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

        private RectTransform _rectTransform;

        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Update()
        {
            _rectTransform.sizeDelta = new(_rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y);
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