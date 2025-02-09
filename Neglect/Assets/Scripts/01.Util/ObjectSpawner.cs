using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Util
{
    public class ObjectSpawner : MonoBehaviour
    {
        public bool isStartSpawn = false; // 이 컴포넌트가 생성되자마자 스폰하게 할 것인지
        [Tooltip("Count와 상관없이 계속 스폰 할 건지")] public bool isLoop = false;
        [Tooltip("생성 잠시 중단")] public bool isPause;
        [Tooltip("스폰이 시작되면 첫 딜레이 없이 바로 스폰할 것인지")] public bool isSpawnImmediate = false;
        [Tooltip("딜레이가 있다면 몇초로 할 것인지")]public float startSpawnDelay = 1f;
        [Tooltip("스폰 간격에 곱샘해준다.")] public float timeScale = 1f;
        
        // 해당 random은 아래의 리스트의 원소상에서의 랜덤임
        public bool isRandomObject = false; // 랜덤한 객체를 소환할 것인지
        public bool isRandomPlace = false; // 랜덤한 위치에 소환할 것인지
        public bool isRandomInterval = false; // 랜덤한 간격에 소환할 것인지
        public Transform parentTransform; // 소환된 객체가 갈 부모 객체 ( 하이어라키에서 관리 편의성을 위해 사용 )
        public MinMaxValue<int> spawnCount = new MinMaxValue<int>(); // 현재 스폰된 갯수

        public List<GameObject> spawnObjectList = new();
        public int[] spawnObjectOrders; // 스폰할 객체, 1개일 경우 해당 객체만 스폰 여러개일 경우 순차적으로 스폰
        private int _SpawnObjectOrderCount = -1; // 현재 스폰할 객체
        private GameObject currentSpawnObject;
    
        [SerializeField] SpawnPlace spawnPlace; // 스폰 위치
        [Tooltip("다음 스폰 위치 순서 인덱스 SpawnPlace를 기반으로 한다.")] public int[] spawnPlaceOrders; // 스폰 위치 설정, 1개일 경우 반복 여러개일 경우 순차적으로 실행
        private int _spawnPlaceCount = -1;
        private Transform _currentSpawnPlace;
    
        public float[] spawnIntervals; // 스폰 간격, 1개일 경우 반복 여러개일 경우 순차적으로 실행
        private int _spawnIntervalCount = -1;
        [HideInInspector] public MinMaxValue<float> intervalTimer = new(true);
    
        public UnityAction<GameObject> SpawnSuccessAction; // 스폰 되면 실행하는 이벤트
        private Coroutine SpawnCoroutine;

        public void Start()
        {
            intervalTimer.isOverMin = true;
            
            spawnPlace.Initialize();
            
            if (isStartSpawn)
            {
                Play();
            }
            
            if (spawnPlace.Length == 0)
            {
                _currentSpawnPlace = gameObject.transform;
            }
        }

        public void OnEnable()
        {
            Play();
        }

        public void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            isPause = false;
            SpawnCoroutine ??= StartCoroutine(SpawnerEnumerator());
        }

        public void Stop()
        {
            StopCoroutine(SpawnCoroutine);
            SpawnCoroutine = null;
        }

        public void Pause()
        {
            isPause = true;
        }

        private IEnumerator SpawnerEnumerator()
        {
            if (!isSpawnImmediate)
                yield return new WaitForSeconds(startSpawnDelay);
            
            while (!spawnCount.IsMax || isLoop)
            {
                if (isPause == false)
                {
                    intervalTimer.Current -= Time.deltaTime * timeScale;
                    if (intervalTimer.IsMin)
                    {
                        Spawn();
                    }
                }
                yield return null;
            } 
        }

        public void Spawn()
        {
            NextObject();
            NextPlace();
            NextInterval();
            var obj = Instantiate(currentSpawnObject, _currentSpawnPlace.position, _currentSpawnPlace.rotation, parentTransform);
            spawnCount.Current++;
        }

        private void NextObject()
        {
            if (isRandomObject)
                _SpawnObjectOrderCount = spawnObjectOrders.Length == 0 ? Random.Range(0, spawnObjectList.Count) : Random.Range(0, spawnObjectOrders.Length);
            else
                _SpawnObjectOrderCount++;

            if (spawnObjectOrders.Length != 0 && spawnObjectOrders.Length - 1 < _SpawnObjectOrderCount) _SpawnObjectOrderCount = 0;
            else if (spawnObjectList.Count - 1 < _SpawnObjectOrderCount) _SpawnObjectOrderCount = 0;
            currentSpawnObject = spawnObjectList[_SpawnObjectOrderCount];
        }
    
        void NextPlace()
        {
            if (spawnPlace.Length == 0)
            {
                _currentSpawnPlace = transform;
                return;
            }
            
            _spawnPlaceCount++;
            int length = 0;
            
            // 길이 할당
            length = spawnPlaceOrders.Length != 0 ? spawnPlaceOrders.Length : spawnPlace.Length;
            
            // 인덱스 설정
            if (isRandomPlace)
            {
                _spawnPlaceCount = Random.Range(0, length);
            }
            if (_spawnPlaceCount >= length) _spawnPlaceCount = 0;
            
            // 위치 할당
            _currentSpawnPlace = spawnPlace.GetSpot(_spawnPlaceCount);
        }
    
        void NextInterval()
        {
            if (spawnIntervals.Length == 0)
            {
                intervalTimer.SetMax(1);
                return;
            }

            float interval = 0;
            
            if (isRandomInterval)
                _spawnIntervalCount =  Random.Range(0, spawnIntervals.Length);
            else
                _spawnIntervalCount++;

            if (_spawnIntervalCount >= spawnIntervals.Length) _spawnIntervalCount = 0;
            
            if (spawnIntervals.Length == 0) interval = 0;
            else interval = spawnIntervals[_spawnIntervalCount];

            var dis = intervalTimer.Current;
            intervalTimer.SetMax(interval);
            intervalTimer.Current += dis;
        }
    }

    [System.Serializable]
    public class SpawnPlace
    {
        [SerializeField] private List<Transform> _spotList = new List<Transform>();
        private Dictionary<string, Transform> _spotDictionary;

        public int Length => _spotList.Count;
        
        public void Initialize()
        {
            _spotDictionary = new Dictionary<string, Transform>();
            foreach (var spot in _spotList)
            {
                AddSpot(spot);
            }
        }

        public Transform AddSpot(Transform spot, string name = null)
        {
            if (name == null)
                _spotDictionary.Add(spot.name, spot);
            else 
                _spotDictionary.Add(name, spot);
            return spot;
        }

        public Transform GetSpot(string name) => _spotDictionary[name];
        public Transform GetSpot(int value) => _spotList.Count > value ? _spotDictionary.Values.ToArray()[value] : null;

        public Transform GetRandomSpot()
        {
            if (_spotDictionary.Count == 0)
                return null;
            
            var r = Random.Range(0, _spotDictionary.Count);
            return GetSpot(r);
        }

        public void SetAllSpotActive(bool value)
        {
            foreach (var (key, spotTransform) in _spotDictionary)
                spotTransform.gameObject.SetActive(value);
        }
    }
}