using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Quest.UI
{
    public class QuestResult : MonoBehaviour
    {
        public ScrollRect scrollRect;
        [Tooltip("퀘스트 결과를 알려주는 텍스트와 아이콘이 있는 프리펩")]public QuestResultBlock questResultBlockPrefab;
        public Sprite ignoreSprite;
        public Sprite completeSprite;

        // 절취선
        private static readonly string lineSTR = "--------------------------------------------------------------------------------";
        
        public void Awake()
        {
            // 테스트 중인 객체 있으면 제거
            for (int i = 0; i < scrollRect.content.childCount; i++)
                Destroy(scrollRect.content.GetChild(i).gameObject);
            
            // 블럭 생성
            var quests = FindObjectsOfType<QuestBase>();
            foreach (QuestBase quest in quests)
                MakeResultBlock(quest);
        }

        public void MakeResultBlock(QuestBase quest)
        {
            var block = Instantiate(questResultBlockPrefab, scrollRect.content);

            block.titleText.text = quest.ToString() + lineSTR;
            block.resultIcon.sprite = quest.state == QuestState.Completed ? completeSprite : ignoreSprite;
        }
    }
}

