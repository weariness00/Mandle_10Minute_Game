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
            ForceUpdate();
        }

        public void ForceUpdate()
        {
            rectTransform = GetComponent<RectTransform>();

            var size = rectTransform.sizeDelta;
            if(size == Vector2.zero) return;
            gridCount.x = (int)((size.x - padding.left) / (cellSize.x + spacing.x));
            gridCount.y = (int)((size.y - padding.top) / (cellSize.y + spacing.y));

            foreach (var data in gridList)
            {
                data.UpdateCellTransform(padding, cellSize, spacing);
            }
        }

        public void Insert(Component uiComponent) => Insert(uiComponent.gameObject);
        public void Insert(Component uiComponent, Vector2Int index) => Insert(uiComponent, index, Vector2Int.one);
        public void Insert(Component uiComponent, Vector2Int index, Vector2Int size) => Insert(uiComponent.gameObject, index, size);
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
            
            // 새로운 데터 삽입
            var newData = new CellData() { index = index, size = size, uiObject = uiObject };
            newData.UpdateIndex(index, gridCount);
            newData.UpdateCellTransform(padding, cellSize, spacing);
            gridList.Insert(findIndex, newData);
            
            // 그리드 겹치면 밀리게 하기
            for (int i = findIndex + 1; i < gridList.Count; i++)
            {
                var data1 = gridList[i - 1];
                var data2 = gridList[i];
                if (data1.CompareTo(data2))
                {
                    data2.UpdateIndex(data2.index + Vector2Int.right, gridCount);
                    data2.UpdateCellTransform(padding, cellSize, spacing);
                }
                else
                    data1.UpdateIndex(data1.index - Vector2Int.right, gridCount);
            }
            gridList.Sort((a, b) =>
            {
                var aIndex = a.index.x + a.index.y * gridCount.x;
                var bIndex = b.index.x + b.index.y * gridCount.x;
                return aIndex.CompareTo(bIndex);
            });
        }

        public void Insert(CellData cellData)
        {
            var index = cellData.index;
            var size = cellData.size;
            var uiObject = cellData.uiObject;
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
            
            // 새로운 데터 삽입
            cellData.UpdateIndex(index, gridCount);
            cellData.UpdateCellTransform(padding, cellSize, spacing);
            gridList.Insert(findIndex, cellData);
            
            // 그리드 겹치면 밀리게 하기
            for (int i = findIndex + 1; i < gridList.Count; i++)
            {
                var data1 = gridList[i - 1];
                var data2 = gridList[i];
                if (data1.CompareTo(data2))
                {
                    data2.UpdateIndex(data2.index + Vector2Int.right, gridCount);
                    data2.UpdateCellTransform(padding, cellSize, spacing);
                }
                else
                    data1.UpdateIndex(data1.index - Vector2Int.right, gridCount);
            }
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
        [Serializable]
        public class CellData
        {
            public Vector2Int index;
            public Vector2Int size;
            public GameObject uiObject;

            public Vector2Int endIndex => index + Vector2Int.one * size - Vector2Int.one;
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

                var originLocalPosition = uiRectTransform.localPosition;
                
                uiRectTransform.anchorMin = Vector2.one * 0.5f;
                uiRectTransform.anchorMax = Vector2.one * 0.5f;
                uiRectTransform.pivot = Vector2.one * 0.5f;

                // 피봇을 움직이면 size만큼 더 움직이게 된다.
                var realSize = cellSize * size + spacing * (size - Vector2Int.one);
                uiRectTransform.localPosition = originLocalPosition + 0.5f * new Vector3(realSize.x, -realSize.y);
            }

            // 각 셀이 겹치는지
            public bool CompareTo(CellData other)
            {
                // 한 사각형이 다른 사각형의 왼쪽에 위치하는 경우
                if (index.x > other.endIndex.x || other.index.x > endIndex.x)
                    return false;

                // 한 사각형이 다른 사각형의 위쪽에 위치하는 경우
                if (endIndex.y < other.index.y || other.endIndex.y < index.y)
                    return false;

                // 겹치는 경우
                return true;
            }
        }
    }
}

