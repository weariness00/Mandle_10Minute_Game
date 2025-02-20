using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Util
{
    /// <summary>
    /// Random을 생성할때 이미 전에 생성된 값은 안나오게 해주는 클래스
    /// </summary>
    public partial class UniqueRandom
    {
        public UniqueRandom(int min, int max)
        {
            Initialize(min, max);
        }

        public UniqueRandom(List<int> list)
        {
            _uniqueIntList = list;
        }
        
        private List<int> _uniqueIntList;

        public bool IsEmptyInt => _uniqueIntList.Count == 0;
        public int RandomInt()
        {
            Debug.Assert(_uniqueIntList != null && _uniqueIntList.Count != 0, "UniqueRandom의 Array를 초기화 하기 위해 먼저 Initialize 메서드를 호출해주세요");
            var index = Random.Range(0, _uniqueIntList.Count);
            var value = _uniqueIntList[index];
            _uniqueIntList.RemoveAt(index);
            return value;
        }

        /// <summary>
        /// [min, max) 를 포함한 랜덤
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Initialize(int min, int max)
        {
            _uniqueIntList = new List<int>();
            for (int i = min; i <= max; i++)
            {
                _uniqueIntList.Add(i);
            }
        }
    }

    public partial class UniqueRandom
    {
        public UniqueRandom(float min, float max, int length, int precision)
        {
            Initialize(min, max, length, precision);
        }
        
        public bool IsEmpty => _uniqueFloatList.Count == 0;

        private List<float> _uniqueFloatList;

        public float RandomFloat()
        {
            Debug.Assert(_uniqueFloatList != null && _uniqueFloatList.Count != 0, "UniqueRandom의 Array를 초기화 하기 위해 먼저 Initialize 메서드를 호출해주세요");
            var index = Random.Range(0, _uniqueFloatList.Count);
            var value = _uniqueFloatList[index];
            _uniqueFloatList.RemoveAt(index);
            return value;
        }

        public void Initialize(float min, float max, int length, int precision)
        {
            _uniqueFloatList = new();
            for (int i = 0; i < length; i++)
            {   
                float factor = Mathf.Pow(10, precision);
                float roundedValue = Mathf.Round(Random.Range(min, max) * factor) / factor;
                _uniqueFloatList.Add(roundedValue);
            }
        }
    }
}