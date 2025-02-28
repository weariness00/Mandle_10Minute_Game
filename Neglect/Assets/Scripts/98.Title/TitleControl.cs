using DG.Tweening;
using GamePlay;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Title
{
    public class TitleControl : MonoBehaviour
    {
        public SpriteRenderer clickSpriteRenderer;
        
        // Update is called once per frame
        private bool isUpdate = true;
        
        void Update()
        {
            if(isUpdate == false) return;   
            
            if (
#if ENABLE_INPUT_SYSTEM
                Mouse.current.leftButton.wasReleasedThisFrame
#else
                Input.GetMouseButtonUp(0)
#endif
            )
            {
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                var hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue);
                Debug.DrawRay(ray.origin, ray.direction * float.MaxValue, Color.blue, 1f);
                if (hit.collider != null && hit.collider.CompareTag("Title Click"))
                {
                    clickSpriteRenderer.DOFade(1f, 3f);
                    Camera.main.DOOrthoSize(1f, 3f).OnComplete(() =>
                    {
                        SceneUtil.LoadReal();
                    });
                    isUpdate = false;
                }
            }
        }
    }
}
