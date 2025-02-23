using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Quest.UI
{
    public class QuestResult : MonoBehaviour
    {
        public Canvas mainCanvas;
        public RectTransform rectTransform;
        public ScrollRect scrollRect;
        [Tooltip("퀘스트 결과를 알려주는 텍스트와 아이콘이 있는 프리펩")]public QuestResultBlock questResultBlockPrefab;
        public Sprite ignoreSprite;
        public Sprite completeSprite;

        [Header("Interface UI 관련")] 
        public Button nextButton;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            // 테스트 중인 객체 있으면 제거
            for (int i = 0; i < scrollRect.content.childCount; i++)
                Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        public void Init()
        {
            // 이미 생성된 블럭 제거
            for (int i = 0; i < scrollRect.content.childCount; i++)
                Destroy(scrollRect.content.GetChild(i).gameObject);
            
            // 블럭 생성
            var quests = QuestManager.Instance.GetAllQuest();
            foreach (QuestBase quest in Enumerable.Reverse(quests).ToArray())
                MakeResultBlock(quest);
        }

        public void MakeResultBlock(QuestBase quest)
        {
            if(quest == null || ReferenceEquals(quest,null)) return;
            
            var block = Instantiate(questResultBlockPrefab, scrollRect.content);

            block.titleText.text = quest.ToString();
            block.resultIcon.sprite = quest.state == QuestState.Completed ? completeSprite : ignoreSprite;
        }
    }
}

