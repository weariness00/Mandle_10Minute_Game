using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GamePlay.Event
{
    public class CallingScreen : MonoBehaviour
    {
        // Start is called before the first frame update
        public Action ClearAction;
        public Action IgnoreAction;
        public TextMeshProUGUI name;

        public void CallAcception()
        {
            if(ClearAction != null)
                ClearAction();
            Destroy(gameObject);
        }
        public void CallRejection()
        {
            if (IgnoreAction !=null)
                IgnoreAction();
            Destroy(gameObject);
        }
    }
}