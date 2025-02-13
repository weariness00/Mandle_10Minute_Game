using UnityEngine;
using Util;

namespace GamePlay
{
    public class GameManager : Singleton<GameManager>
    {
        [Tooltip("방해 이벤트를 초기화(시작)했는지")] public bool isInitQuest = false;
        
        public void Awake()
        {
            if(!SceneUtil.TryGetPhoneScene(out var scene))
                SceneUtil.AsyncAddPhone();
        }
    }
}

