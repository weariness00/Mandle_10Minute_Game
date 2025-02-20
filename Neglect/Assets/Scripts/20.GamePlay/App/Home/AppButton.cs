using System;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace GamePlay.App
{
    public class AppButton : MonoBehaviour
    {
        public Button button;
        public GameObject highlightObject;

        public void Awake()
        {
            highlightObject.SetActive(false);
        }
    }
}

