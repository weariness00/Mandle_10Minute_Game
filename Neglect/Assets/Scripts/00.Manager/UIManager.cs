using UnityEngine;
using Util;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas phoneCanvas;

        public static T InstantiateFromPhone<T>(T obj) where T : Object
        {
            return Instantiate(obj, Instance.phoneCanvas.transform);
        }
    }
}

