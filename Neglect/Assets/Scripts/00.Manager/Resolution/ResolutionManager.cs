using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Manager
{
    public partial class ResolutionManager : Singleton<ResolutionManager>
    {
        public bool isDebug = true;
        public ResolutionSetting setting;
        public Vector2 currentResolutionSize;

        public ResolutionSetting.InstantiateObject objects;

        private static readonly string ScreenModeKey = "ResolutionScreenMode";

        public void Awake()
        {
            setting = ResolutionSettingProviderHelper.setting;
            Debug.Assert(setting != null, $"{nameof(ResolutionSetting)}가 존재하지 않습니다");
            if(ReferenceEquals(setting, null)) return;
            objects = setting.Instantiate();
            objects.resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);

            if (PlayerPrefs.HasKey("ResolutionWidth"))
            {
                var size = new Vector2(PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight"));
                OnResolutionChange(size);
            }
            else
                OnResolutionChange(0);

            ScreenModeInit();
        }

#if UNITY_EDITOR
        public void Update()
        {
            EditorScreenUpdate();
        }
#endif

        public void OnResolutionChange(Vector2 size)
        {
            int index = setting.FindResolutionSizeIndex(size);
            if (index == objects.resolutionDropdown.value) OnResolutionChange(index);
            objects.resolutionDropdown.value = setting.FindResolutionSizeIndex(size);
        }
        public void OnResolutionChange(Int32 index)
        {
            if(isDebug) Debug.Log($"[{setting.GetSize(index).ToString()}]으로 해상도 변경");
            currentResolutionSize = setting.GetSize(index).ToVector2();
            foreach (var scaler in FindObjectsOfType<ResolutionCanvasScaler>())
            {
                scaler.ScaleUpdate(currentResolutionSize);
            }
            PlayerPrefs.SetInt("ResolutionWidth", (int)currentResolutionSize.x);
            PlayerPrefs.SetInt("ResolutionHeight", (int)currentResolutionSize.y);

#if UNITY_WEBGL
            StartCoroutine(ChangeScreenEnumerator((int)currentResolutionSize.x, (int)currentResolutionSize.y));
#elif UNITY_STANDALONE
            Screen.SetResolution((int)currentResolutionSize.x, (int)currentResolutionSize.y, Screen.fullScreen);
#endif
        }

        private void ScreenModeInit()
        {
            if (setting.screenModeType == ResolutionSetting.ScreenModeType.EveryScreen)
            {
                objects.screenModeDropdown.AddOptions(new List<string>(){nameof(FullScreenMode.Windowed),nameof(FullScreenMode.ExclusiveFullScreen), nameof(FullScreenMode.FullScreenWindow)});
            }
            else if (setting.screenModeType == ResolutionSetting.ScreenModeType.FullScreen)
            {
                objects.screenModeDropdown.AddOptions(new List<string>(){nameof(FullScreenMode.Windowed), nameof(FullScreenMode.FullScreenWindow)});
            }
            objects.screenModeDropdown.onValueChanged.AddListener(OnScreenModeChange);

            if (PlayerPrefs.HasKey(ScreenModeKey))
            {
                OnScreenModeChange(PlayerPrefs.GetInt(ScreenModeKey));
            }
        }
        
        public void OnScreenModeChange(int value)
        {
#if UNITY_WEBGL
            Screen.fullScreen = value != 0;
#elif UNITY_STANDALONE
            if (setting.screenModeType == ResolutionSetting.ScreenModeType.EveryScreen)
            {
                if (value == 0)
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                else if(value == 1)
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                else if(value == 2)
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    
                PlayerPrefs.SetInt(ScreenModeKey, value);
            }
            else if (setting.screenModeType == ResolutionSetting.ScreenModeType.FullScreen)
            {
                if (value == 0)
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                else if(value == 1)
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    
                PlayerPrefs.SetInt(ScreenModeKey, value);
            }
#endif
            if(isDebug) Debug.Log($"[{Screen.fullScreenMode}]으로 화면 모드 변경");
        }
    }

#if UNITY_WEBGL
    public partial class ResolutionManager
    {
        private IEnumerator ChangeScreenEnumerator(int w, int h)
        {
            var screenMode = Screen.fullScreen;
            Screen.SetResolution((int)currentResolutionSize.x, (int)currentResolutionSize.y, true);
            yield return null;
            Screen.SetResolution((int)currentResolutionSize.x, (int)currentResolutionSize.y, screenMode);
        }
    }
#endif
    
#if UNITY_EDITOR
    public partial class ResolutionManager
    {
        private Vector2 editorGameViewSize;
        private void EditorScreenUpdate()
        {
            var size = new Vector2(Screen.width, Screen.height);
            if (editorGameViewSize != size)
            {
                editorGameViewSize = size;
                OnResolutionChange(size);
            }
        }
    }
#endif
}

