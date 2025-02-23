using DG.Tweening;
using MoreMountains.Feedbacks;
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

        public MMF_Player RoadingMMF;
        public MMF_Player ShowButtonMMF;
        public MMF_Player HideObjectMMF;


        public Action Complete;
        public Action Ignore;

        public int chance = 3; // 0 이상일 경우 안될 가능성이 있음;

        public void Start() //스타트 단계에서 setactive 해야지 ui가 인지됨
        {
            Roading.SetActive(true);
            ReConnectPopup.SetActive(false);
        }
        public void DelayComplete()
        {
            int randomvalue = UnityEngine.Random.Range(1, 11);
            chance--;
            if (chance == 2)
            {
                if (randomvalue >= 5)
                {

                    Roading.SetActive(true);
                    ReConnectPopup.SetActive(false);
                    RoadingMMF.PlayFeedbacks();

                    return;
                }
            }
            else if (chance == 1)
            {
                if (randomvalue >= 7)
                {

                    Roading.SetActive(true);
                    ReConnectPopup.SetActive(false);
                    RoadingMMF.PlayFeedbacks();
                    return;
                }
            }
            Destroy(gameObject);
            Complete();
        }

        public void Reconnect()
        {
            
            HideObjectMMF.PlayFeedbacks();
        }
    }
}