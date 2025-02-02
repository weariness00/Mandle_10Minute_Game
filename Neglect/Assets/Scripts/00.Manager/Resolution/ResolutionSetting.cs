using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Manager
{

    [CreateAssetMenu(fileName = "Resolution Setting Data", menuName = "Manager/Resolution", order = 0)]
    public partial class ResolutionSetting : ScriptableObject
    {
        [Tooltip("해상도 UI가 생성될 메인 Canvas")]public Canvas canvasPrefab;
        [Tooltip("해상도 설정을 닫아줄 버튼")]public Button exitButtonPrefab;
        [Tooltip("해상도 설정 버튼")]public TMP_Dropdown resolutionDropdownPrefab;
        public ScreenModeType screenModeType;
        [Tooltip("스크린 모드 Dropdown")] public TMP_Dropdown screenModeDropdownPrefab;
        [Tooltip("해상도 사이즈 리스트")]public List<ResolutionSize> resolutionList = new();
        
        public InstantiateObject Instantiate(
            Transform parentTransform = null,
            bool isActiveExitUI = true)
        {
            var canvas = parentTransform == null ? Instantiate(canvasPrefab) : Instantiate(canvasPrefab, parentTransform);
            var exitButton = Instantiate(exitButtonPrefab, canvas.transform);
            var dropdown = Instantiate(resolutionDropdownPrefab, canvas.transform);
            var screenModeDropdown = Instantiate(screenModeDropdownPrefab, canvas.transform);
            
            exitButton.onClick.AddListener(() => canvas.gameObject.SetActive(false));
            exitButton.gameObject.SetActive(isActiveExitUI);
            dropdown.AddOptions(resolutionList.Select(r => r.ToString()).ToList());
            screenModeDropdown.ClearOptions();

            var objects = new InstantiateObject();
            objects.canvas = canvas;
            objects.resolutionDropdown = dropdown;
            objects.screenModeDropdown = screenModeDropdown;
            return objects;
        }

        public ResolutionSize GetSize(int index) =>  index < resolutionList.Count ? resolutionList[index] : new ResolutionSize(1920, 1080);
        public ResolutionSize FindSize(Vector2 size) => resolutionList.DefaultIfEmpty(resolutionList[0]).FirstOrDefault(r => r.Width == (int)size.x && r.Height == (int)size.y);

        public int FindResolutionSizeIndex(Vector2 size)
        {
            for (int i = 0; i < resolutionList.Count; i++)
            {
                if (resolutionList[i].ToVector2() == size) return i;
            }

            return -1;
        }
        
    }

    public partial class ResolutionSetting
    {
        [Serializable]
        public class InstantiateObject
        {
            public Canvas canvas;
            public TMP_Dropdown resolutionDropdown;
            public TMP_Dropdown screenModeDropdown;
        }

        public enum ScreenModeType
        {
            FullScreen,
            EveryScreen, // 창, 전체, 테두리 없음 전체, 테두리 없음 창
        }
        
        [Serializable]
        public struct ResolutionSize
        {
            public int Width;
            public int Height;

            public ResolutionSize(int w, int h)
            {
                Width = w;
                Height = h;
            }

            public Vector2 ToVector2()
            {
                return new Vector2(Width, Height);
            }

            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }
    }
}