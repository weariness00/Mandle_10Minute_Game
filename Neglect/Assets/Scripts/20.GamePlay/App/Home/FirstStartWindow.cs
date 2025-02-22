using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.App
{
    public class FirstStartWindow : MonoBehaviour
    {
        public TMP_Text dayText;
        public TMP_Text timeText;
        public EventTrigger eventTrigger;
        public EventTrigger.Entry clickEntry;
        
        private TimeZoneInfo kstZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

        public void Awake()
        {
            if (eventTrigger == null)
                eventTrigger = gameObject.AddComponent<EventTrigger>();
            
            clickEntry = eventTrigger.triggers.FirstOrDefault(e => e.eventID == EventTriggerType.PointerClick);
            if (clickEntry == null)
                clickEntry = new EventTrigger.Entry() { eventID = EventTriggerType.PointerClick };
            clickEntry.callback.AddListener(data => OnPointerClick((PointerEventData)data));
            
            eventTrigger.triggers.Add(clickEntry);
        }

        public void Update()
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kstZone);
            string dateOnly = now.ToString("yyyy-MM-dd"); // 날짜만 (예: 2025-02-22)
            string timeOnly = now.ToString("HH:mm");      // 시간만 (예: 14:30)
            dayText.text = dateOnly;
            timeText.text = timeOnly;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Destroy(gameObject);
        }
    }
}

