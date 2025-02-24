using DG.Tweening;
using DG.Tweening.Core;
using GamePlay.Phone;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GamePlay.Event
{
    public class ChargerConnect : MonoBehaviour
    {
        public GameObject Charger;
        public GameObject chargerLine;
        public ChargerHead head;
        public PhoneControl phone;

        public GameObject drag;
        public bool isClear; 
        public bool isDrag;

        public bool inTarget;

        public Action ClearAction;

        public MMF_Player ResetPostion;    


        Vector3 offset;


        public void Update()
        {
            if (isClear)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                offset = Charger.transform.position - new Vector3(worldPos.x, worldPos.y, 30);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if(hit.collider!=null&& hit.collider.gameObject == Charger)
                    isDrag = true;
            }
            if (isDrag)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                
                Charger.transform.position = offset + new Vector3(worldPos.x, worldPos.y, 30) ;
            }

            if (!isClear&& Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (head.inPort)
                {
                    isClear = true;
                    EventClaer();
                }
                else
                {
                    isDrag = false;
                    ResetPostion.PlayFeedbacks();
                }
            }
        }


        public void EventClaer()
        { 
            Charger.transform.DOMove(phone.ChargingPort.transform.position, 0.5f).OnComplete(()=> {
                if(ClearAction!=null)
                    ClearAction();
            });
        }

        public void HideAnimation()
        {
            Sequence seq = DOTween.Sequence();
            SpriteRenderer ChargerRender = Charger.GetComponent<SpriteRenderer>();
            SpriteRenderer ChargerLineRender = chargerLine.GetComponent<SpriteRenderer>();
            seq.Append(ChargerLineRender.DOFade(0f, 0.5f));
            seq.Join(ChargerRender.DOFade(0f, 0.5f)).OnComplete(() => {
                Destroy(gameObject);
            });
        }

    }
}