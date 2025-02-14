using System;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Object = UnityEngine.Object;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas sharedCanvas;

        protected override void Initialize()
        {
            base.Initialize();
            var obj = new GameObject("Shared Canvas");
            sharedCanvas = obj.AddComponent<Canvas>();
            
            DontDestroyOnLoad(obj);
        }

        public static T InstantiateUI<T>(T uiObject) where T : Object
        {
            var obj = GameObject.Instantiate(uiObject, Instance.sharedCanvas.transform);
            return obj;
        }

    }
}

