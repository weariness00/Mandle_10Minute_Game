using System;
using Manager;
using UnityEngine;

namespace GamePlay
{
    /// <summary>
    /// 환경 설정 컨트롤 용
    /// </summary>
    public class SettingControl : MonoBehaviour
    {
        [Tooltip("셋팅에 포함될 메인 Canvas")]public Canvas mainCanvas;
        
        public void Awake()
        {
            Destroy(ResolutionManager.Instance.objects.canvas);
            ResolutionManager.Instance.objects = ResolutionManager.Instance.setting.Instantiate(mainCanvas.transform, false);
        }
    }
}

