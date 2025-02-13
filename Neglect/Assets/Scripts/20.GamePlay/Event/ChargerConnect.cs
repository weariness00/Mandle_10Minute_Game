using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GamePlay.Event
{
    public class ChargerConnect : MonoBehaviour
    {
        [Header("오브젝트 할당")]
        public RectTransform Charger;          //충전기 
   
        public bool isClear; // 이벤트를 완수 했는가?
        public bool isDrag;

        public bool inTarget;

        public Action ClearAction;

        [Header("MMF 애니메이션")]
        public MMF_Player ResetPostion;      // 충전기 복귀 애니메이션
        public MMF_Player TargetPostion;     // 충전기 완료 애니메이션

        public RectTransform canvasRect;

        public void Update()
        {
            if (isClear)
                return;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (!inTarget)
                {
                    isDrag = false;
                    ResetPostion.PlayFeedbacks();
                }
                else
                {
                    isClear = true;
                    EventClaer();
                }
            }
            if (isDrag)
            {
                Charger.anchoredPosition = GetMousePositionInCanvas();
            }

        }
        public Vector2 GetMousePositionInCanvas()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue(); // 마우스 위치 (스크린 좌표)

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,  // 캔버스의 RectTransform
                mousePos,     // 현재 마우스 스크린 위치
                Camera.main,         // 카메라 (World Space Canvas라면 Camera.main)
                out localPoint
            );
            return localPoint; // UI 내에서의 로컬 좌표 반환
        }

        public void ClickCharger()
        {
            isDrag = true;
        }

        public void TargetEnter()
        {
            if(isDrag)
                inTarget = true;
        }
        public void TargetExit()
        {
            inTarget = false ;
        }


        public void EventClaer()
        {
            TargetPostion.PlayFeedbacks();

            ClearAction();
        }

        public void HideAnimation()
        {
            Destroy(gameObject);
        }

    }
}