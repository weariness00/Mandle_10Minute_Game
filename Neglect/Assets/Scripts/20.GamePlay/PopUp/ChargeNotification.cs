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
        public MMF_Player ChargingStartMMF;
        public Action IgnoreAction;
        public PhoneControl phone;

        public TextMeshProUGUI BetteryText;
        public TextMeshProUGUI ChargingText;
        public int energy;
        public AudioSource ChargeCompleteSound;
        
        public void OnEnable()
        {
            ShowNotificationMMF.PlayFeedbacks();    
        }

        public void SettingBetteryText(int bettery)
        {
            energy = bettery;
            BetteryText.text = bettery.ToString() +"%";
            ChargingText.text = "배터리가 부족합니다.\n충전해주세요.";
        }

        public void ChargingStart() //충전기가 complete 작동 -> chartgingStart - > hideanimation -> destory
        {
            BetteryText.gameObject.SetActive(false);
            ChargingText.text = "충전중" + energy +"%\n";
            ChargingStartMMF.PlayFeedbacks();
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

        public void ChargeCompleteSoundPlay()
        {
            ChargeCompleteSound.Play();
        }

        public void SelfDestory()
        {
            Destroy(gameObject);
        }
    }
}