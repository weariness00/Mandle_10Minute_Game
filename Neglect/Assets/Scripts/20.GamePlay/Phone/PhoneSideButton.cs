using Manager;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GamePlay.Phone
{
    public class PhoneSideButton : MonoBehaviour
    {
        public PhoneControl phone;
        public UnityEvent onClickEvent;
    
        public virtual void Awake()
        {
            InputManager.game.PhoneClick.performed += ButtonClick;
        }

        public void OnDestroy()
        {
            if (InputManager.HasInstance)
            {
                InputManager.game.PhoneClick.performed -= ButtonClick;
            }
        }

        public void ButtonClick(InputAction.CallbackContext context)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePos);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue);
            if (hit.transform == transform)
            {
                onClickEvent?.Invoke();
            }
        }
    }
}

