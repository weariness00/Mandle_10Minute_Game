using System;
using UnityEngine;
using Util;

namespace Manager
{
    public partial class InputManager : Singleton<InputManager>
    {
        public void Awake()
        {
            InitRunningInput();
        }
    }
    
    public partial class InputManager
    {
        private Running _running;
        public static Running running => Instance._running;
        public class Running
        {
            public readonly RunningInput input = new();
            public Vector2 MovePosition => input.Player.Move.ReadValue<Vector2>();
            public bool SlidingDown => input.Player.Sliding.ReadValue<float>() > 0f;
        }

        public void InitRunningInput()
        {
            _running = new();
        }
    }
}

