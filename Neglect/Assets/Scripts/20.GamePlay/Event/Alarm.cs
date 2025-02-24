using GamePlay.Phone;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GamePlay.Event
{
    public class Alarm : MonoBehaviour
    {
        public Action complete;
        public Action ignore;
        public TextMeshProUGUI timeText;
        public PhoneControl phone;
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
            if(ignore != null) 
                ignore();
            SelfDestroy();
        }

        public void SelfDestroy()
        {

            Destroy(gameObject);
        }
    }
}