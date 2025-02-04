using UnityEngine;

namespace Quest
{
    [CreateAssetMenu(fileName = "Quest Data", menuName = "Game/Quest", order = 0)]
    public class QuestScriptableObject : ScriptableObject
    {
        public int id = -1;
        public string questName;
        public QuestBase questPrefab;
        public QuestLevel level;
        [Tooltip("해당 퀘스트가 종료되면 시작할 후속 퀘스트")] public int nextQuestId = -1;

        public QuestBase Instantiate(Transform parent = null)
        {
            var quest = Instantiate(questPrefab, parent);
            quest.nextQuestID = nextQuestId;

            return quest;
        }
    }
}