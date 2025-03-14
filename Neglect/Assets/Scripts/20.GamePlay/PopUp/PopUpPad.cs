﻿using MoreMountains.Feedbacks;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Util;

namespace GamePlay.PopUp
{
    public partial class PopUpPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        public bool isClick;
        [Tooltip("얼마만큼 움직여야 팝업이 제거되는지")] public Vector2 destroyMoveDistance = new(300,100);
        [Tooltip("팝업이 제거될때 동작 하는 이벤트")] public UnityEvent destroyPopUpEvent;

        public MinMaxValue<float> destroyTimer = new(0,0,15);

        [Header("UI Object")] 
        public Button button;
        public TMP_Text titleText;
        public TMP_Text explainText;

        private RectTransform rectTransform;
        private bool isX;
        private bool isY;
        private Vector3 originPosition;
        
        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            spawnFeel = GetComponentInChildren<MMF_Player>();
        }

        public void Start()
        {
            rectTransform.anchoredPosition = Vector2.zero;
            
            button.onClick.AddListener(() => Destroy(gameObject));
        }

        public void Update()
        {
            destroyTimer.Current += Time.deltaTime;
            if (destroyTimer.IsMax)
            {
                destroyPopUpEvent?.Invoke();
                Destroy(gameObject);
            }
        }

        public void OnEnable()
        {
            var pos = transform.position;
            pos.y = rectTransform.sizeDelta.y;
            
            MMFInit();
            spawnFeel.PlayFeedbacks();
        }

#if UNITY_EDITOR
        public void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            spawnFeel = GetComponentInChildren<MMF_Player>();
            
            Debug.Assert(spawnFeel != null, $"{nameof(MMF_Player)} 컴포넌트가 존재하지 않습니다.");
        }
        
        public void OnValidate()
        {
            MMFInit();
        }
#endif

        public void OnPointerDown(PointerEventData eventData)
        {
            isClick = true;
            isX = false;
            isY = false;

            originPosition = transform.localPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isClick = false;

            if ((isX && Vector3.Distance(transform.localPosition, originPosition) > destroyMoveDistance.x) ||
                (isY && Vector3.Distance(transform.localPosition, originPosition) > destroyMoveDistance.y))
            {
                // 사라지는 연출
                destroyPopUpEvent?.Invoke();
                // 현재는 없어서 그냥 바로 파괴
                Destroy(gameObject);
            }
            else
            {
                transform.localPosition = originPosition;
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
                    transform.localPosition += new Vector3(eventData.delta.x, 0, 0);
                else if(isY)
                    transform.localPosition += new Vector3(0, eventData.delta.y, 0);
            }
        }
    }
    
    public partial class PopUpPad
    {
        [Header("MMF 관련")]
        private MMF_Player spawnFeel;
        private string mmfPositionLabel = "Spawn Position";
        private MMF_Position mmfPosition;

        private void MMFInit()
        {
            if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if(spawnFeel == null) spawnFeel = GetComponentInChildren<MMF_Player>();
            if(mmfPosition == null) mmfPosition = new(){Label = mmfPositionLabel};
            var mmfPos = spawnFeel.GetFeedbacksOfType<MMF_Position>().FirstOrDefault(p => p.Label == mmfPositionLabel);
            if (mmfPos == null) spawnFeel.AddFeedback(mmfPosition);
            else mmfPosition = mmfPos;
   
            mmfPosition.Space = MMF_Position.Spaces.RectTransform;
            mmfPosition.RelativePosition = false;
            mmfPosition.AnimatePositionTarget = gameObject;
            mmfPosition.InitialPosition = Vector3.zero;
            mmfPosition.DestinationPosition = Vector3.zero;
            mmfPosition.DestinationPosition.y = -rectTransform.sizeDelta.y / 2;
            mmfPosition.AnimatePositionDuration = 1f;
        }
    }
}

