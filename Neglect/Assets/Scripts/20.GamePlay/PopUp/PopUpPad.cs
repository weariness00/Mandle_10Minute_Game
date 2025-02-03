using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GamePlay.PopUp
{
    public class PopUpPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        public bool isClick;
        [Tooltip("얼마만큼 움직여야 팝업이 제거되는지")] public Vector2 destroyMoveDistance = new(300,100);
        [Tooltip("팝업이 제거될때 동작 하는 이벤트")] public UnityEvent destroyPopUpEvent;

        [Header("UI Object")] 
        public Button leftButton;
        public Button rightButton;
        public Image viewIcon;
        public TMP_Text viewExplain;
        
        private bool isX;
        private bool isY;
        private Vector3 originPosition;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isClick = true;
            isX = false;
            isY = false;

            originPosition = transform.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isClick = false;

            if ((isX && Vector3.Distance(transform.position, originPosition) > destroyMoveDistance.x) ||
                (isY && Vector3.Distance(transform.position, originPosition) > destroyMoveDistance.y))
            {
                // 사라지는 연출
                destroyPopUpEvent?.Invoke();
                // 현재는 없어서 그냥 바로 파괴
                Destroy(gameObject);
            }
            else
            {
                transform.position = originPosition;
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isClick)
            {
                if (!isX && !isY)
                {
                    var x = eventData.delta.x;
                    var y = eventData.delta.y;
                    if (Mathf.Abs(x) > y)
                    {
                        isX = true;
                    }
                    else if(y > 0)
                    {
                        isY = true;
                    }
                }
                else if(isX)
                    transform.position += new Vector3(eventData.delta.x, 0, 0);
                else if(isY)
                    transform.position += new Vector3(0, eventData.delta.y, 0);
                
            }
        }
    }
}

