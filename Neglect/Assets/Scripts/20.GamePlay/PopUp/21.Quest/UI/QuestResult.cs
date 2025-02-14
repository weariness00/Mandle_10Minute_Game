using Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quest.UI
{
    public class QuestResult : MonoBehaviour
    {
        [Tooltip("퀘스트 결과를 알려주는 텍스트와 아이콘이 있는 프리펩")]public GameObject questResultUIBlockPrefab;
        public Sprite ignoreSprite;
        public Sprite completeSprite;
        
        public void Awake()
        {
            var quests = FindObjectsOfType<QuestBase>();
            foreach (QuestBase quest in quests)
            {
                MakeResultBlock(quest);
            }
        }

        public void MakeResultBlock(QuestBase quest)
        {
            var obj = UIManager.InstantiateUI(questResultUIBlockPrefab);
            var explain = obj.GetComponentInChildren<TMP_Text>();
            var icon = obj.GetComponentInChildren<Image>();

            explain.text = quest.ToString();
            icon.sprite = completeSprite;
        }
    }
}

