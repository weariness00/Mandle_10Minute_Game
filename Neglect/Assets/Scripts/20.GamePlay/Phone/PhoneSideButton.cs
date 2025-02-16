using Manager;
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

        public void ButtonClick(InputAction.CallbackContext context)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject == gameObject)
                {
                    onClickEvent?.Invoke();
                }
            }
        }
    }
}

