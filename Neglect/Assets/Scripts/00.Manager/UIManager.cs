using System;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Object = UnityEngine.Object;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public GameObject phoneCanvas;

        protected override void Initialize()
        {
            base.Initialize();
            phoneCanvas = GameObject.FindWithTag("Phone Canvas");
        }

        public static T InstantiateFromPhone<T>(T obj) where T : Object
        {
            return Instantiate(obj, Instance.phoneCanvas.transform, false);
        }

        public static RawImage InstantiateRenderTextureImage(int width, int height)
        {
            var obj = new GameObject("Render Texture Raw Image");
            var rawImage = obj.AddComponent<RawImage>();

            obj.transform.SetParent(Instance.phoneCanvas.transform, false);
            rawImage.rectTransform.sizeDelta = new Vector2(width, height);
            
            return rawImage;
        }
    }
}

