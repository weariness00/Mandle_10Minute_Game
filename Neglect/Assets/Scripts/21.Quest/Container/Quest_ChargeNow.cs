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
    public Vector3 SpawnPos;
    public override void OnNext(object value)
    {

    }

  
    public override void Play()
    {
        base.Play();

        var ChargerPopup = UIManager.InstantiateFromPhone(notification);
        ChargerPopup.gameObject.SetActive(true);
        var Charger = Instantiate(charger , SpawnPos , transform.rotation);
        Charger.gameObject.SetActive(true);

        Charger.ClearAction += Complete;
        ChargerPopup.IgnoreAction += Ignore;
    }

    private void Update()
    {

    }

    public override void Complete()
    {
        base.Complete();
   
    }

    public override void Ignore()
    {
        base.Ignore();
        
    }
}
