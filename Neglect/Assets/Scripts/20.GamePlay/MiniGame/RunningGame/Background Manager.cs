using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Util;
using static UnityEngine.GraphicsBuffer;

namespace GamePlay.MiniGame.RunningGame
{
    public class BackgroundManager : MonoBehaviour
    {

        public bool IsPause; //퍼즈인지 
        public float LeftPosX;



        [Header("생성할 배경")]
        public List<GameObject> BackgroundPrefab = new List<GameObject>(); // 생성할 배경 프리팹
        public List<GameObject> MiddlegroundPrefab = new List<GameObject>(); // 생성할 중경 프리팹
        public List<GameObject> ForegroundPrefab = new List<GameObject>(); // 생성할 전경 프리팹
        public List<GameObject> FloorPrefab = new List<GameObject>(); // 생성할 바닥 프리팹

        [Header("생성된 배경")]
        public List<GameObject> BackgroundObject = new List<GameObject>(); // 생성된 배경 이미지
        public List<GameObject> MiddlegroundObject = new List<GameObject>(); // 생성된 중경 이미지
        public List<GameObject> ForegroundObject = new List<GameObject>(); // 생성된 전경 이미지
        public List<GameObject> FloorObject = new List<GameObject>(); // 생성된 바닥 프리팹

        [Header("생성된 배경 크기")]
        private List<Vector2> BackgroundSize = new List<Vector2>(); // 생성된 배경 크기
        private List<Vector2> MiddlegroundSize = new List<Vector2>(); // 생성된 중경 크기
        private List<Vector2> ForegroundSize = new List<Vector2>(); // 생성된 전경 크기
        private List<Vector2> FloorSize = new List<Vector2>(); // 생성된 바닥 크기

        [Header("배경 속도")]
        public MinMaxValue<int> BackgroundSpeed = new MinMaxValue<int>(); //배경이미지 속도
        public MinMaxValue<int> MiddlegroundSpeed = new MinMaxValue<int>(); //중경이미지 속도
        public MinMaxValue<int> ForegroundSpeed = new MinMaxValue<int>(); //전경이미지 속도
        public MinMaxValue<int> FloorSpeed = new MinMaxValue<int>(); //바닥이미지 속도


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
            spawn();
        }

        public void ScreenSizeCalcul() //임시 최대 경계선 계산
        {
            float yScreenHalfSize = Camera.main.orthographicSize;
            float xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;
            LeftPosX = -(xScreenHalfSize)*(1.5f);
        }

        public void spawn()
        {
            Vector3 SpawnPos = BackgroundPos;
            while (BackgroundObject.Count < BackgroundCount) // 최대 갯수를 채울만큼 반복
            {
                for (int i = 0; i < BackgroundPrefab.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
                {
                    SpriteRenderer spriteRenderer = BackgroundPrefab[i].GetComponent<SpriteRenderer>();
                    Vector2 pixelSize = spriteRenderer.bounds.size;
                    BackgroundSize.Add(pixelSize);
                    SpawnPos.x += pixelSize.x / 2;
                    var obj = Instantiate(BackgroundPrefab[i],SpawnPos, Quaternion.identity, BackParent.transform);
                    BackgroundObject.Add(obj);
                    SpawnPos.x += pixelSize.x / 2;
                }
            }

            SpawnPos = MiddlegroundPos;
            while (MiddlegroundObject.Count < MiddlegroundCount) // 최대 갯수를 채울만큼 반복
            {
                for (int i = 0; i < MiddlegroundPrefab.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
                {
                    SpriteRenderer spriteRenderer = MiddlegroundPrefab[i].GetComponent<SpriteRenderer>();
                    Vector2 pixelSize = spriteRenderer.bounds.size;
                    MiddlegroundSize.Add(pixelSize);
                    SpawnPos.x += pixelSize.x / 2;
                    var obj = Instantiate(MiddlegroundPrefab[i], SpawnPos, Quaternion.identity, MidParent.transform);
                    MiddlegroundObject.Add(obj);
                    SpawnPos.x += pixelSize.x/2;
                }
            }

            SpawnPos = ForegroundPos;
            while (ForegroundObject.Count < ForegroundCount) // 최대 갯수를 채울만큼 반복
            {
                for (int i = 0; i < ForegroundPrefab.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
                {
                    SpriteRenderer spriteRenderer = ForegroundPrefab[i].GetComponent<SpriteRenderer>();
                    Vector2 pixelSize = spriteRenderer.bounds.size;
                    ForegroundSize.Add(pixelSize);
                    SpawnPos.x += pixelSize.x / 2 ;
                    var obj = Instantiate(ForegroundPrefab[i], SpawnPos, Quaternion.identity, ForeParent.transform);
                    ForegroundObject.Add(obj);
                    SpawnPos.x += pixelSize.x / 2 + ForegroundInterval;
                }
            }

            SpawnPos = FloorPos;
            while (FloorObject.Count < FloorCount) // 최대 갯수를 채울만큼 반복
            {
                for (int i = 0; i < FloorPrefab.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
                {
                    SpriteRenderer spriteRenderer = FloorPrefab[i].GetComponent<SpriteRenderer>();
                    Vector2 pixelSize = spriteRenderer.bounds.size;
                    FloorSize.Add(pixelSize);
                    SpawnPos.x += pixelSize.x / 2;
                    var obj = Instantiate(FloorPrefab[i], SpawnPos, Quaternion.identity, FloorParent.transform);
                    FloorObject.Add(obj);
                    SpawnPos.x += pixelSize.x / 2 + FloorInterval;
                }
            }
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
                if (BackgroundObject[i].transform.position.x < LeftPosX)
                {
                    int LastIndex = (i - 1) < 0 ? BackgroundObject.Count - 1 : i - 1;

                    Vector3 pre = BackgroundObject[LastIndex].transform.position;
                    pre.x += BackgroundSize[LastIndex].x / 2 + BackgroundSize[i].x / 2 + BackgroundInterval; // 좌표계산
                    BackgroundObject[i].transform.position = pre;
                }
                BackgroundObject[i].transform.position += new Vector3(-BackgroundSpeed, 0, 0) * Time.deltaTime * RunningGame.GameSpeed;
                
            }

            for (int i = 0; i < MiddlegroundObject.Count; i++)
            {
                if (MiddlegroundObject[i].transform.position.x < LeftPosX)
                {

                    int LastIndex = (i - 1) < 0 ? MiddlegroundObject.Count - 1 : i - 1;
                    Vector3 pre = MiddlegroundObject[LastIndex].transform.position;

                    pre.x += MiddlegroundSize[LastIndex].x / 2 + MiddlegroundSize[i].x / 2 + MiddlegroundInterval; // 좌표계산
                    MiddlegroundObject[i].transform.position = pre;
                }
                MiddlegroundObject[i].transform.position += new Vector3(-MiddlegroundSpeed, 0, 0) * Time.deltaTime * RunningGame.GameSpeed;
            }

            for (int i = 0; i < ForegroundObject.Count; i++)
            {
                if (ForegroundObject[i].transform.position.x < LeftPosX)
                {
                    int LastIndex = (i - 1) < 0 ? ForegroundObject.Count - 1 : i - 1;
                    Vector3 pre = ForegroundObject[LastIndex].transform.position;
                    pre.x += ForegroundSize[LastIndex].x / 2 + ForegroundSize[i].x / 2 + ForegroundInterval; // 좌표계산
                    ForegroundObject[i].transform.position = pre;
                }
                ForegroundObject[i].transform.position += new Vector3(-ForegroundSpeed, 0, 0) * Time.deltaTime * RunningGame.GameSpeed;
            }

            for (int i = 0; i < FloorObject.Count; i++)
            {
                if (FloorObject[i].transform.position.x < LeftPosX)
                {
                    int LastIndex = (i - 1) < 0 ? FloorObject.Count - 1 : i - 1;
                    Vector3 pre = FloorObject[LastIndex].transform.position;
                    pre.x += FloorSize[LastIndex].x / 2 + FloorSize[i].x / 2 + FloorInterval; // 좌표계산
                    FloorObject[i].transform.position = pre;
                }
                FloorObject[i].transform.position += new Vector3(-FloorSpeed, 0, 0) * Time.deltaTime * RunningGame.GameSpeed;
            }
        }
    }
}