using System;
using Manager;
using UniRx;
using UnityEngine;

namespace GamePlay.MiniGame.RunningGame
{
    // 멤버 변수
    // 유니티 이벤트 함수
    public partial class RunningGame : MiniGameBase
    {
        public static float GameSpeed = 1f;
        public PlayerData[] playerDataArray = new PlayerData[3];

        public override void Awake()
        {
            base.Awake();
            GameSpeed = 1f;
            
            InputManager.running.input.Enable();
        }

        public void ChangeSpeed(float speed)
        {
            GameSpeed = speed;
        }
    }

    
    public partial class RunningGame
    {
        public PlayerData GetPlayerData() => playerDataArray[0];
    }

    public partial class RunningGame
    {
        [Serializable]
        public class PlayerData
        {
            public ReactiveProperty<int> score = new(0);
            public string name;
        }
    }

    
}

