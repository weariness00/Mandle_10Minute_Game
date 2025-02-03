
using System;
using Manager;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    public class RunningGame : MiniGameBase
    {
        public static float GameSpeed = 1f;

        public override void Awake()
        {
            base.Awake();
            GameSpeed = 1f;
            
            InputManager.running.input.Enable();
        }
    }
}

