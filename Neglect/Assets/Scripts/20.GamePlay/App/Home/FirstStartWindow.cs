﻿using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.App
{
    public class FirstStartWindow : MonoBehaviour
    {
        public EventTrigger eventTrigger;
        public EventTrigger.Entry clickEntry;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            Destroy(gameObject);
        }
    }
}

