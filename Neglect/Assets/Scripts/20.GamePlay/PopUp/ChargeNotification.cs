using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GamePlay.PopUp
{
    public class ChargeNotification : MonoBehaviour, IPointerDownHandler
    {

        public MMF_Player ShowNotificationMMF; //알림 show
        public MMF_Player HideNotificationMMF; //알림 hide
        public Action IgnoreAction; 
        public void OnEnable()
        {
            ShowNotificationMMF.PlayFeedbacks();    
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HideNotificationMMF.PlayFeedbacks();
            IgnoreAction();
        }

        public void ChargerConnectToPhone() //휴대폰이 연결 되었을때
        {
            HideNotificationMMF.PlayFeedbacks();
        }

        public void HideAnimation()
        {
            Destroy(gameObject);
        }
    }
}