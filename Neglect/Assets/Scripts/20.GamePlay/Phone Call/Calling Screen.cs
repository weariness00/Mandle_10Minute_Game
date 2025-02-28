using GamePlay.Phone;
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

        [HideInInspector] public PhoneControl phone;
        public void Update()
        {
            if (!ReferenceEquals(phone, null) && phone.viewType == PhoneViewType.Horizon)
            {
                phone.FadeOut(0f, Color.black);
                phone.PhoneViewRotate(PhoneViewType.Vertical, () => phone.FadeIn(1f, Color.black));
            }
        }

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