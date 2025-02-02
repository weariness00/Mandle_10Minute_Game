using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class ResolutionCanvasScaler : MonoBehaviour
    {
        private CanvasScaler CanvasScaler;

        public void Awake()
        {
            CanvasScaler = GetComponent<CanvasScaler>();
            CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            ScaleUpdate(ResolutionManager.Instance.currentResolutionSize);
        }

        public void ScaleUpdate(Vector2 resolution)
        {
            CanvasScaler.referenceResolution = resolution;
        }
    }
}

