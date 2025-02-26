using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamePlay.App.Home
{
    /// <summary>
    /// 앱 버튼 위치 컨트롤 해주는 그리드
    /// </summary>
    public partial class AppGridControl : MonoBehaviour
    {
        [HideInInspector] public RectTransform rectTransform;
        public RectOffset padding;
        public Vector2 spacing;
        public Vector2 cellSize = new Vector2(100, 100); // 셀 1칸당 크기

        [HideInInspector] public Vector2Int gridCount;// 그리드 갯수

        private List<CellData> gridList = new();
        
        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnRectTransformDimensionsChange()
        {
            rectTransform = GetComponent<RectTransform>();

            gridCount.x = (int)((rectTransform.sizeDelta.x - padding.left) / (cellSize.x + spacing.x));
            gridCount.y = (int)((rectTransform.sizeDelta.y - padding.top) / (cellSize.y + spacing.y));

            foreach (var data in gridList)
            {
                
                data.UpdateCellTransform(padding, cellSize, spacing);
            }
        }

        public void Insert(Component uiComponent) => Insert(uiComponent.gameObject);
        public void Insert(Component uiComponent, Vector2Int index) => Insert(uiComponent.gameObject, index, Vector2Int.one);
        public void Insert(GameObject uiObject)
        {
            if(gridList.Count == 0)
                Insert(uiObject, Vector2Int.zero, Vector2Int.one);
            else
            {
                var index = gridList[^1].index + Vector2Int.right;
                // index = index.x >= gridCount.x ? new Vector2Int(0, index.y + 1) : index;
                Insert(uiObject, index, Vector2Int.one);
            }
        }
        
        public void Insert(GameObject uiObject, Vector2Int index, Vector2Int size)
        {
            var findIndex = gridList.Count;
            for (int i = 0; i < gridList.Count; i++)
            {
                var data = gridList[i];
                if (data.index == index)
                {
                    findIndex = i;
                    break;
                }
            }

            for (int i = findIndex; i < gridList.Count; i++)
            {
                var data = gridList[i];
                data.UpdateIndex(data.index + Vector2Int.right, gridCount);
                data.UpdateCellTransform(padding, cellSize, spacing);
            }
            
            var newData = new CellData() { index = index, size = size, uiObject = uiObject };
            newData.UpdateIndex(index, gridCount);
            newData.UpdateCellTransform(padding, cellSize, spacing);
            gridList.Add(newData);
            gridList.Sort((a, b) =>
            {
                var aIndex = a.index.x + a.index.y * gridCount.x;
                var bIndex = b.index.x + b.index.y * gridCount.x;
                return aIndex.CompareTo(bIndex);
            });
        }
        
        public bool TryAdd(Component uiComponent, Vector2Int index) => TryAdd(uiComponent.gameObject, index, Vector2Int.one);
        public bool TryAdd(GameObject uiObject, Vector2Int index) => TryAdd(uiObject, index, Vector2Int.one);
        public bool TryAdd(GameObject uiObject, Vector2Int index, Vector2Int size)
        {
            // 인덱스 범위를 넘어가면
            if(index.x * size.x >= gridCount.x || index.y * size.y >= gridCount.y) return false;
            var findGrid = gridList.Find(c =>
                c.index.x <= index.x && index.x <= c.endIndex.x &&
                c.index.y <= index.y && index.y <= c.endIndex.y
            );

            // 추가하려는 곳에 이미 다른 Cell이 존재하면
            if (findGrid != null)
                return false;

            var data = new CellData() { index = index, size = size, uiObject = uiObject };
            data.UpdateCellTransform(padding, cellSize, spacing);

            gridList.Add(data);
            gridList.Sort((a, b) =>
            {
                var aIndex = a.index.x + a.index.y * gridCount.x;
                var bIndex = b.index.x + b.index.y * gridCount.x;
                return aIndex.CompareTo(bIndex);
            });
            return true;
        }
    }

    public partial class AppGridControl
    {
        public class CellData
        {
            public Vector2Int index;
            public Vector2Int size;
            public GameObject uiObject;

            public Vector2Int endIndex => index * size;

            public void UpdateIndex(Vector2Int _index, Vector2Int gridCount)
            {
                // var newindex = _index + Vector2Int.right;
                // newindex = newindex.x >= gridCount.x ? new Vector2Int(0, newindex.y + 1) : newindex;
                index = _index;
            }
            
            public void UpdateCellTransform(RectOffset padding, Vector2 cellSize, Vector2 spacing)
            {
                var uiRectTransform = uiObject.GetComponent<RectTransform>();
                uiRectTransform.anchorMin = Vector2.up;
                uiRectTransform.anchorMax = Vector2.up;
                uiRectTransform.pivot = Vector2.up;
                uiRectTransform.sizeDelta = cellSize * size + spacing * (size - Vector2Int.one);
                uiRectTransform.anchoredPosition = cellSize * index + spacing * index + new Vector2(padding.left, padding.top);
                uiRectTransform.anchoredPosition *= new Vector2(1,-1);
            }
        }
    }
}

