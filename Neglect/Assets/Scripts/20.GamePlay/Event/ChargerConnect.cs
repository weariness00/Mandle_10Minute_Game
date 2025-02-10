using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay.Event
{
    public class ChargerConnect : MonoBehaviour
    {
        [Header("오브젝트 할당")]
        public GameObject Charger;          //충전기 
        public GameObject ChargerHead;      // 충전기 끝부분
        [Tooltip("충전기 목표 및 시작 위치")]
        public GameObject Target;           // 충전기 드래그 끝 부분
        public GameObject InitPostion;      // 충전기 드래그 시작부분
        private Vector3 offset;
        [Tooltip("충전기 판정 범위")]
        public Vector2 DectectedRange = new Vector2(5, 10); // 충전기 판정
        public bool isDragging; // 드래그 중인가?
        public bool isClear; // 이벤트를 완수 했는가?


        [Header("MMF 애니메이션")]
        public MMF_Player ResetPostion;      // 충전기 복귀 애니메이션
        public MMF_Player TargetPostion;     // 충전기 완료 애니메이션


        public void Update()
        {

            if (Mouse.current.leftButton.wasPressedThisFrame && !isClear)
            {
                isDragging = true;
                float chargerZ = Camera.main.WorldToScreenPoint(Charger.transform.position).z;
                Vector2 pointerPosition = Mouse.current.position.ReadValue();
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, chargerZ));

                offset = Charger.transform.localPosition -  worldPosition;
            }

            if (isDragging && !isClear)
            {
                Vector2 pointerPosition = Mouse.current.position.ReadValue();
                float chargerZ = Camera.main.WorldToScreenPoint(Charger.transform.position).z;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, chargerZ));
                Charger.transform.localPosition = worldPosition +offset;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
            {
                if (IsChargerNearTarget()) // 성공 조건 충전기가 충전단자에 근접한지
                    EventClaer();                //클리어시 작동될 코드
                else                             
                    ResetPostion.PlayFeedbacks(); //위치 제자리
                isDragging = false;
            }

        }

        public bool CheckClickCharger(Vector3 ClickPos)
        {
            return true;
        }

        public void EventClaer()
        {
            isClear = true;
            TargetPostion.PlayFeedbacks();
        }

        public bool IsChargerNearTarget()
        {
            if (DectectedRange.x >= ChargerHead.transform.position.x - Target.transform.position.x)
            {
                if (DectectedRange.y >= Mathf.Abs(ChargerHead.transform.position.y - Target.transform.position.y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}