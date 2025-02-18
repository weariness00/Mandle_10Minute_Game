using GamePlay.Phone;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GamePlay.PopUp
{
    public class ChargeNotification : MonoBehaviour
    {

        public MMF_Player ShowNotificationMMF; //알림 show
        public MMF_Player HideNotificationMMF; //알림 hide
        public Action IgnoreAction;
        public PhoneControl phone;

        public TextMeshProUGUI BetteryText;
        public void OnEnable()
        {
            ShowNotificationMMF.PlayFeedbacks();    
        }

        public void SettingBetteryText(string bettery)
        {
            BetteryText.text = bettery +"%";
        }

        public void ChargerIgone()
        {
            HideNotificationMMF.PlayFeedbacks();
            IgnoreAction();
        }

        public void HideAnimation()
        {
            
            HideNotificationMMF.PlayFeedbacks();
        }

        public void SelfDestory()
        {
            Destroy(gameObject);
        }
    }
}