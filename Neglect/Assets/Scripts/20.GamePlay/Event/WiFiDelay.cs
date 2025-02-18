using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    public class WiFiDelay : MonoBehaviour
    {
        public GameObject ReConnectPopup; //팝업창
        public GameObject Roading; //로딩 애니메이션

        public Action Complete;
        public Action Ignore;

        public void OnEnable()
        {
            Roading.SetActive(true);
            ReConnectPopup.SetActive(false);
        }
        public void DelayComplete()
        {
            Complete();
            Destroy(gameObject);
        }
    }
}