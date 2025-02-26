using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Util
{
    [Serializable]
    public class CurveMovement2D
    {
        [Tooltip("Move를 적용할 객체 Transform")] public Transform transform;
        [Tooltip("그래프에 따라 오브젝트가 움직인다.")]public AnimationCurve curve;

        [Space] 
        [Tooltip("그래프의 xy 배율")] public Vector2 curveMagnification = Vector2.one;
        [Tooltip("이동 속도")] public float speed = 1f;
        [Tooltip("시간 가속")] public float timeScale = 1f;
        
        [Space]
        [Tooltip("마지막 키에 도달하면 위치 업데이트를 할 것인지")] public bool isUpdatePositionLastKey = true;
        [Tooltip("회전을 할 것인지")] public bool isRotate = true;
        [Tooltip("그래프 X축의 반대 방향으로 움직일 것인지")] public bool isOppositeX;
        [Tooltip("그래프 Y축의 반대 방향으로 움직일 것인지")] public bool isOppositeY;

        [Tooltip("Curve의 마지막 키에 도달하면 호출")]public UnityEvent OnLastKey;
        
        [HideInInspector] public bool isChangeDir = true; // 진행 방향이 바뀌었는지
        private float elapsedTime;
        private Vector3 up;
        private Vector3 right;
        private Vector3 originPosition;

        public float Duration => curve.keys.LastOrDefault().time; // curve의 총 시간
        
        public void Init()
        {
            if (isChangeDir)
            {
                up = transform.up;
                right = transform.right;
                isChangeDir = false;

                if (isOppositeX)
                    right = -right;
                
                if (isOppositeY)
                    up = -up;
            }
            originPosition = transform.position;
            elapsedTime = 0;
        }

        public void Move()
        {
            float normalize = (elapsedTime * timeScale) % curve.keys.LastOrDefault().time;
            float curveValue = curve.Evaluate(normalize);
            
            // 위치 변경
            var dir = normalize * curveMagnification.x * right + curveValue * curveMagnification.y * up;
            var newPos = originPosition + dir;
            newPos.z = transform.position.z;
            
            // 회전 변경
            if (isRotate)
            {
                var dirNormal = (transform.position - newPos).normalized;
                var angle = Mathf.Atan2(dirNormal.y, dirNormal.x) * Mathf.Rad2Deg;
                var rotation = transform.rotation;
                
                //회전 적용
                transform.rotation = Quaternion.Euler(rotation.x, rotation.y, angle);
            }
            
            // 위치 적용
            transform.position = newPos;
            
            elapsedTime += Time.deltaTime * speed;
            if(elapsedTime * timeScale > Duration)
            {
                OnLastKey?.Invoke();
                if(isUpdatePositionLastKey) Init();
            }
        }
    }
}

