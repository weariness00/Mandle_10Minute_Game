using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GamePlay.Chatting
{
    public class ChatTextBox : MonoBehaviour
    {
        // Start is called before the first frame update
        public RectTransform boxTransform;
        public Image image;
        [SerializeField] private TMP_Text targetText;

        public RectOffset padding;
        public float maxWidth;

        private RectTransform _rectTransform;

        public void Awake()
        {
            targetText.enableWordWrapping = true; // 자동 줄바꿈 활성화
        }

        public void OnEnable()
        {
            UpdateBoxSize();
        }

        public void UpdateBoxSize()
        {
            _rectTransform = GetComponent<RectTransform>();
            
            float width = targetText.preferredWidth;
            float height = targetText.preferredHeight;

            if (width > maxWidth)
            {
                targetText.rectTransform.sizeDelta = new Vector2(maxWidth, height);
                width = Mathf.Min(width, maxWidth);
                height = targetText.preferredHeight; // 줄바꿈 후 높이 다시 계산
            }
            
            targetText.rectTransform.sizeDelta = new Vector2(width, height);
            targetText.rectTransform.anchoredPosition = new Vector3(padding.left, -padding.top);
            image.rectTransform.sizeDelta = new Vector2(width + padding.horizontal, height + padding.vertical);
            _rectTransform.sizeDelta = new(_rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y);
        }

        public void SetText(string text)
        {
            this.targetText.text = text;
            UpdateBoxSize();
            UpdateBoxSize();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChatTextBox))]
    public class ChatTextBoxEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (EditorApplication.isPlaying)
            {
                var script = target as ChatTextBox;
                if (GUILayout.Button("대화 박스 크기 업데이트"))
                {
                    script.UpdateBoxSize();
                }
            }
        }
    }
#endif
}