
using UnityEngine;

namespace GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public void Awake()
        {
            SceneUtil.AsyncAddPhone();
        }
    }
}

