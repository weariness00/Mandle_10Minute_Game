using System;
using UnityEngine;
using Util;
using Object = UnityEngine.Object;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas phoneCanvas;

        protected override void Initialize()
        {
            base.Initialize();
            phoneCanvas = GameObject.FindWithTag("Phone Canvas").GetComponent<Canvas>();
        }

        public static T InstantiateFromPhone<T>(T obj) where T : Object
        {
            return Instantiate(obj, Instance.phoneCanvas.transform);
        }
    }
}

