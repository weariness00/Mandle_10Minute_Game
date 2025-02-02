using System;
using Manager;
using UnityEngine;

namespace GamePlay
{
    public class InitControl : MonoBehaviour
    {
        private void Awake()
        {
            var resolutionManager = ResolutionManager.Instance;
        }
    }
}

