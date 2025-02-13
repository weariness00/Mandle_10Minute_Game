using System.Collections.Generic;
using UnityEngine;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public class BackgroundManager : MonoBehaviour
    {
        public RunningGame runningGame;
        public bool IsPause; //퍼즈인지 
        public float LeftPosX;

        [Header("생성할 배경")]
        public List<GroundObject> BackgroundPrefab = new List<GroundObject>(); // 생성할 배경 프리팹
        public List<GroundObject> MiddlegroundPrefab = new List<GroundObject>(); // 생성할 중경 프리팹
        public List<GroundObject> ForegroundPrefab = new List<GroundObject>(); // 생성할 전경 프리팹
        public List<GroundObject> FloorPrefab = new List<GroundObject>(); // 생성할 바닥 프리팹

        [Header("생성된 배경")]
        public List<GroundObject> BackgroundObject = new List<GroundObject>(); // 생성된 배경 이미지
        public List<GroundObject> MiddlegroundObject = new List<GroundObject>(); // 생성된 중경 이미지
        public List<GroundObject> ForegroundObject = new List<GroundObject>(); // 생성된 전경 이미지
        public List<GroundObject> FloorObject = new List<GroundObject>(); // 생성된 바닥 프리팹

        [Header("생성된 배경 크기")]
        private List<Vector2> BackgroundSize = new List<Vector2>(); // 생성된 배경 크기
        private List<Vector2> MiddlegroundSize = new List<Vector2>(); // 생성된 중경 크기
        private List<Vector2> ForegroundSize = new List<Vector2>(); // 생성된 전경 크기
        private List<Vector2> FloorSize = new List<Vector2>(); // 생성된 바닥 크기

        [Header("배경 속도 ")]
        public MinMaxValue<int> BackgroundSpeed = new MinMaxValue<int>(); //배경이미지 속도
        public MinMaxValue<int> MiddlegroundSpeed = new MinMaxValue<int>(); //중경이미지 속도
        public MinMaxValue<int> ForegroundSpeed = new MinMaxValue<int>(); //전경이미지 속도
        public MinMaxValue<int> FloorSpeed = new MinMaxValue<int>(); //바닥이미지 속도



        [Header("전경 구름 속도")]
        public MinMaxValue<int> CloseFogSpeed = new MinMaxValue<int>(); 
        public MinMaxValue<int> MiddleFogSpeed = new MinMaxValue<int>(); 
        public MinMaxValue<int> FarFogSpeed = new MinMaxValue<int>(); 

        [Header("배경 간격")]
        public MinMaxValue<int> BackgroundInterval = new MinMaxValue<int>(); //배경이미지 간격
        public MinMaxValue<int> MiddlegroundInterval = new MinMaxValue<int>(); //중경이미지 간격
        public MinMaxValue<int> ForegroundInterval = new MinMaxValue<int>(); //전경이미지 간격
        public MinMaxValue<int> FloorInterval = new MinMaxValue<int>(); //바닥이미지 간격


        [Header("배경 최대 수")]
        public MinMaxValue<int> BackgroundCount = new MinMaxValue<int>(); //배경이미지 최대 갯수
        public MinMaxValue<int> MiddlegroundCount = new MinMaxValue<int>(); //중경이미지 최대 갯수
        public MinMaxValue<int> ForegroundCount = new MinMaxValue<int>(); //전경이미지 최대 갯수


        public MinMaxValue<int> FloorCount = new MinMaxValue<int>(); //바닥이미지 간격


        [Header("배경이미지 소환 위치")]
        public Vector3 BackgroundPos = new Vector3(); //배경이미지 소환 위치
        public Vector3 MiddlegroundPos = new Vector3(); //중경이미지 소환 위치
        public Vector3 ForegroundPos = new Vector3(); //전경이미지 소환 위치
        public Vector3 FloorPos = new Vector3(); //전경이미지 소환 위치

        [Header("Instantiate 부모 위치")]
        public GameObject BackParent;
        public GameObject MidParent;
        public GameObject ForeParent;
        public GameObject FloorParent;

        public void OnEnable()
        {
            Play();
        }
        public void OnDisable()
        {
            Stop();
        }

        // Start is called before the first frame update
        public void Play()
        {
            ScreenSizeCalcul();
            IsPause = false;
            Spawn();
        }

        public void ScreenSizeCalcul() //임시 최대 경계선 계산
        {
            float yScreenHalfSize = Camera.main.orthographicSize;
            float xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;
            LeftPosX = -(xScreenHalfSize);
        }

        public void Spawn()
        {
            void PrefabSpawn(List<GroundObject> spawnObjectList, List<GroundObject> prefabList, List<Vector2> sizeList, Vector3 SpawnPos, Transform parent, int spawnLength, float speed)
            {
                while (spawnObjectList.Count < spawnLength) // 최대 갯수를 채울만큼 반복
                {
                    foreach (var prefab in prefabList)
                    {
                        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
                        Vector2 pixelSize = spriteRenderer.bounds.size;
                        sizeList.Add(pixelSize);
                        SpawnPos.x += pixelSize.x / 2;
                        var obj = Instantiate(prefab, SpawnPos, Quaternion.identity, parent);
                        obj.Setting(speed);
                        obj.runningGame = runningGame;
                        obj.gameObject.layer = LayerMask.NameToLayer("Phone");
                        spawnObjectList.Add(obj);
                        SpawnPos.x += pixelSize.x / 2;
                    }
                }
            }

            PrefabSpawn(BackgroundObject, BackgroundPrefab, BackgroundSize, BackgroundPos, BackParent.transform, BackgroundCount, BackgroundSpeed);
            PrefabSpawn(MiddlegroundObject, MiddlegroundPrefab, MiddlegroundSize, MiddlegroundPos, MidParent.transform, MiddlegroundCount, BackgroundSpeed);

            {
                var SpawnPos = ForegroundPos;
                while (ForegroundObject.Count < ForegroundCount) // 최대 갯수를 채울만큼 반복
                {
                    for (int i = 0; i < ForegroundPrefab.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
                    {
                        SpriteRenderer spriteRenderer = ForegroundPrefab[i].GetComponent<SpriteRenderer>();
                        Vector2 pixelSize = spriteRenderer.bounds.size;
                        ForegroundSize.Add(pixelSize);
                        SpawnPos.x += pixelSize.x / 2;
                        var obj = Instantiate(ForegroundPrefab[i], SpawnPos, Quaternion.identity, ForeParent.transform);
                        obj.runningGame = runningGame;
                        obj.gameObject.layer = LayerMask.NameToLayer("Phone");
                        ForeFogSetting(obj);
                        ForegroundObject.Add(obj);
                        SpawnPos.x += pixelSize.x / 2 + ForegroundInterval;
                    }
                }
            }

            { 
                var SpawnPos = FloorPos;
                while (FloorObject.Count < FloorCount) // 최대 갯수를 채울만큼 반복
                {
                    for (int i = 0; i < FloorPrefab.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
                    {
                        SpriteRenderer spriteRenderer = FloorPrefab[i].GetComponent<SpriteRenderer>();
                        Vector2 pixelSize = spriteRenderer.bounds.size;
                        FloorSize.Add(pixelSize);
                        SpawnPos.x += pixelSize.x / 2;
                        var obj = Instantiate(FloorPrefab[i], SpawnPos, Quaternion.identity, FloorParent.transform);
                        obj.Setting(FloorSpeed);
                        obj.runningGame = runningGame;
                        obj.gameObject.layer = LayerMask.NameToLayer("Phone");
                        FloorObject.Add(obj);
                        SpawnPos.x += pixelSize.x / 2 + FloorInterval;
                    }
                }
            }
        }

        public void ForeFogSetting(GroundObject target)
        {
            int randomValue = Random.Range(0, 3); // 0 가깝고 흐릿함 / 1 중간 선명 //2 느리고 어두움  
            Vector3 Pos = target.transform.position;
            
            if (randomValue == 0)
            {
                Pos.z = 1f;
                target.Setting(Random.Range(CloseFogSpeed.Min, CloseFogSpeed.Max), randomValue);
            }
            else if (randomValue == 1)
            {
                Pos.z = 1.1f;
                target.Setting(Random.Range(MiddleFogSpeed.Min, MiddleFogSpeed.Max), randomValue);
            }
            else if (randomValue == 2)
            {
                Pos.z = 1.2f;
                target.Setting(Random.Range(FarFogSpeed.Min, FarFogSpeed.Max), randomValue);
            }
            target.transform.position = Pos;
        }
        public void Stop()
        {
            IsPause = true;
        }

        public void Update()
        {
            if (IsPause)
                return;
            GroundMove();
        }

        public void GroundMove()
        {

            for (int i = 0; i < BackgroundObject.Count; i++)
            {
                if (BackgroundObject[i].transform.position.x < LeftPosX + BackgroundSize[i].x/4)
                {
                    int LastIndex = (i - 1) < 0 ? BackgroundObject.Count - 1 : i - 1;  //가장 마지막 순번에 위치한 오브젝트 뒤로
                    Vector3 pre = BackgroundObject[LastIndex].transform.position;
                    pre.x += BackgroundSize[LastIndex].x / 4 + BackgroundInterval;
                    BackgroundObject[i].transform.position = pre;
                }
            }

            for (int i = 0; i < MiddlegroundObject.Count; i++)
            {
                if (MiddlegroundObject[i].transform.position.x < LeftPosX - MiddlegroundSize[i].x)
                {
                    int LastIndex = (i - 1) < 0 ? MiddlegroundObject.Count - 1 : i - 1;//가장 마지막 순번에 위치한 오브젝트 뒤로
                    Vector3 pre = MiddlegroundObject[LastIndex].transform.position;

                    pre.x += MiddlegroundSize[LastIndex].x / 2 + MiddlegroundSize[i].x / 2 + MiddlegroundInterval; // 좌표계산
                    MiddlegroundObject[i].transform.position = pre;
                }
            }

            for (int i = 0; i < ForegroundObject.Count; i++)
            {
                if (ForegroundObject[i].transform.position.x < LeftPosX - ForegroundSize[i].x/2)
                {
                    ForeFogSetting(ForegroundObject[i]);
                    ForegroundObject[i].transform.position += Vector3.left * LeftPosX*3;
                }
            }

            for (int i = 0; i < FloorObject.Count; i++)
            {
                if (FloorObject[i].transform.position.x < LeftPosX - FloorSize[i].x)
                {
                    int LastIndex = (i - 1) < 0 ? FloorObject.Count - 1 : i - 1;
                    Vector3 pre = FloorObject[LastIndex].transform.position;
                    pre.x += FloorSize[LastIndex].x / 2 + FloorSize[i].x / 2 + FloorInterval; 
                    FloorObject[i].transform.position = pre;
                }
            }
        }
    }
}