using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Manager
{
    public partial class InputManager : Singleton<InputManager>
    {
        public void Awake()
        {
            _running = new();
            _flapping = new();
            
            _running.Init();
            _flapping.Init();
        }
    }
    
    public partial class InputManager
    {
        private Running _running;
        public static Running running => Instance._running;
        public class Running
        {
            public RunningInput input;
            public InputAction Sliding => input.Player.Sliding;
            public InputAction ESC => input.Stop.ESC;
            
            public Vector2 MovePosition => input.Player.Move.ReadValue<Vector2>();
            public bool SlidingDown => input.Player.Sliding.ReadValue<float>() > 0f;
            
            public void Init()
            {
                input = new();
                input.Enable();
            }
        }
    }

    public partial class InputManager
    {
        private Flapping _flapping;
        public static Flapping flapping => Instance._flapping;
        public class Flapping
        {
            public FlappingInput input;
            public bool IsJump => input.Player.Jump.ReadValue<float>() > 0f;

            public void Init()
            {
                input = new();
                input.Enable();
            }
        }
    }
}

