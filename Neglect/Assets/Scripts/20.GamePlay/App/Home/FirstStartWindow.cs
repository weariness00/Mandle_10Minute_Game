using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.App
{
    public class FirstStartWindow : MonoBehaviour
    {
        public EventTrigger eventTrigger;

        public void Awake()
        {
            if (eventTrigger == null)
                eventTrigger = gameObject.AddComponent<EventTrigger>();
            
            var click = eventTrigger.triggers.FirstOrDefault(e => e.eventID == EventTriggerType.PointerClick);
            if (click == null)
                click = new EventTrigger.Entry() { eventID = EventTriggerType.PointerClick };
            click.callback.AddListener(data => OnPointerClick((PointerEventData)data));
            
            eventTrigger.triggers.Add(click);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var narration =  FindObjectOfType<GamePlayerNarration>(true);
            if (narration)
            {
                narration.StartNarration();
            }
            Destroy(gameObject);
        }
    }
}

