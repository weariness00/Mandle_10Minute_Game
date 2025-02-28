using GamePlay.Phone;
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
        
        [HideInInspector] public PhoneControl phone;

        public void Update()
        {
            if (!ReferenceEquals(phone, null) && phone.viewType == PhoneViewType.Horizon)
            {
                phone.FadeOut(0f, Color.black);
                phone.PhoneViewRotate(PhoneViewType.Vertical, () => phone.FadeIn(1f, Color.black));
            }
        }
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