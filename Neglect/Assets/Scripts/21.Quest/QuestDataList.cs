using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quest
{
    [CreateAssetMenu(fileName = "Quest Data List", menuName = "Game/Quest List", order = 0)]
    public class QuestDataList : ScriptableObject
    {
        [SerializeField] private List<QuestScriptableObject> questList;
        // [SerializeField] private List<QuestScriptableObject> easyQuestList;
        // [SerializeField] private List<QuestScriptableObject> normalQuestList;
        // [SerializeField] private List<QuestScriptableObject> hardQuestList;
        // [SerializeField] private List<QuestScriptableObject> hiddenQuestList;

        public QuestScriptableObject GetQuestID(int id)
        {
            return questList.First(q => q.id == id);
        }

        public QuestScriptableObject GetQuestName(string questName)
        {
            return questList.First(q => q.questName == questName);
        }
    }
}