using GamePlay.Event;
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
    public override void OnNext(object value)
    {

    }

  
    public override void Play()
    {
        base.Play();

        ChargerPopup = PhoneUtil.InstantiateUI(notification);
        ChargerPopup.gameObject.SetActive(true);
        Charger = Instantiate(charger , SpawnPos , transform.rotation);
        Charger.gameObject.SetActive(true);

        Charger.ClearAction += Complete;
        ChargerPopup.IgnoreAction += Ignore;
    }

    private void Update()
    {

    }

    public override void Complete()
    {
        DeleteObject();
        base.Complete();
    }

    public override void Ignore()
    {
        DeleteObject();
        base.Ignore();        
    }
    public void DeleteObject()
    {
        Charger.HideAnimation();
        ChargerPopup.HideAnimation();
    }
}
