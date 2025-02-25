using GamePlay.Event;
using GamePlay.Phone;
using GamePlay.PopUp;
using Manager;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Quest_ChargeNow : QuestBase
{
    public ChargeNotification notification;
    public ChargerConnect charger;

    public ChargeNotification ChargerPopup;
    public ChargerConnect Charger;
    public Vector3 SpawnPos;
    
    private PhoneControl phone;
    private IPhoneApplication app;
    
    public override void OnNext(object value)
    {

    }
    
    public override void Play()
    {
        base.Play();

        if (eventData.extraDataIDArray.Length > 1) isLoop = eventData.extraDataIDArray[1] == -45;
        
        ChargerPopup = PhoneUtil.InstantiateUI(notification, out var phone_);
        ChargerPopup.gameObject.SetActive(true);
        Charger = Instantiate(charger , SpawnPos , transform.rotation);
        phone = phone_;
        Charger.phone = phone;
        Charger.gameObject.SetActive(true);
        phone.PhoneViewRotate(PhoneViewType.Horizon);

        app = phone.applicationControl.currentPlayApplication;
        phone.applicationControl.PauseApp(app);
        app.SetActiveBackground(true);
   
        ChargerPopup.SettingBetteryText(15);
        Charger.ClearAction += Complete;
        ChargerPopup.IgnoreAction += Ignore;
    }

    public override void Complete()
    {
        base.Complete();
        ChargingStart();
        phone.applicationControl.OpenApp(app);

    }

    public override void Ignore()
    {
        base.Ignore();
        DeleteObject();
        phone.applicationControl.OpenApp(app);
    }

    public override void Failed()
    {
        base.Failed();
        DeleteObject();
    }

    public void DeleteObject()
    {
        Charger.HideAnimation();
        ChargerPopup.HideAnimation();
    }
    public void ChargingStart()
    {
        ChargerPopup.ChargingStart();
        Charger.HideAnimation();
    }
}
