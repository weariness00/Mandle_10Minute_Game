using Manager;
using System;
using UnityEngine;

namespace GamePlay
{
    public class PhoneCanvas : MonoBehaviour
    {
        public void Awake()
        {
            UIManager.Instance.phoneCanvas = GetComponent<Canvas>();
        }
    }
}

