using System;
using TMPro;
using UnityEngine;

namespace GamePlay.Event
{
    public class Alarm : MonoBehaviour
    {
        public Action complete;
        public Action ignoreEvent;
        public TextMeshProUGUI timeText;
        public void TimeSet(string text)
        {
            timeText.text = text;
        }
        public void ClearEvent()
        {
            if(complete != null) 
                complete();
            SelfDestroy();
        }

        public void IgnoreEvent()
        {
            if (ignoreEvent != null)
            {
                ignoreEvent?.Invoke();
            }
            SelfDestroy();
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
}