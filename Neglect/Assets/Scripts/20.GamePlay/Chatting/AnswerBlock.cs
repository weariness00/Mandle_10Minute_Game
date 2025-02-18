using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Chatting
{
    public class AnswerBlock : MonoBehaviour
    {
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
        
        public Button button;
        public Image image;
        [Tooltip("답변")]public TMP_Text answerText;
        [Tooltip("답변에 따른 게이지")]public int gage;

        [Tooltip("긍정 or 부정인지")] public bool isPositive;
    }
}

