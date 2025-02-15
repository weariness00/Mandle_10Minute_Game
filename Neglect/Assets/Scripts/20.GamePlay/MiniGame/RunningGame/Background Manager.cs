using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace GamePlay.MiniGame.RunningGame
{
    public class BackgroundManager : MonoBehaviour
    {
        public RunningGame runningGame;
        public bool IsPause; //퍼즈인지 
        public float LeftPosX;

        [Header("생성할 배경")]
        public GroundObject BackgroundPrefab; // 생성할 배경 프리팹
        public List<GroundObject> ForegroundPrefab = new List<GroundObject>(); // 생성할 전경 프리팹
        public GroundObject FloorPrefab; // 생성할 바닥 프리팹

        [Header("생성된 배경")]
        public GroundObject BackgroundObject; // 생성된 배경 이미지
        public List<GroundObject> ForegroundObject = new List<GroundObject>(); // 생성된 전경 이미지
        public GroundObject FloorObject; // 생성된 바닥 프리팹

        [Header("구름 스프라이트 랜더러")]
        public List<SpriteRenderer> ForegroundRender = new List<SpriteRenderer>(); 


        [Header("생성된 배경 크기")]
        public Vector2 BackgroundSize; // 생성된 배경 크기
        public List<Vector2> ForegroundSize = new List<Vector2>(); // 생성된 전경 크기
        public Vector2 FloorSize; // 생성된 바닥 크기

        [Header("배경 속도 ")]
        public MinMaxValue<int> BackgroundSpeed = new MinMaxValue<int>(); //배경이미지 속도
        public MinMaxValue<int> ForegroundSpeed = new MinMaxValue<int>(); //전경이미지 속도
        public MinMaxValue<int> FloorSpeed = new MinMaxValue<int>(); //바닥이미지 속도



        [Header("전경 구름 속도")]
        public MinMaxValue<int> CloseFogSpeed = new MinMaxValue<int>(); 
        public MinMaxValue<int> MiddleFogSpeed = new MinMaxValue<int>(); 
        public MinMaxValue<int> FarFogSpeed = new MinMaxValue<int>(); 

        [Header("배경 간격")]
        public MinMaxValue<int> BackgroundInterval = new MinMaxValue<int>(); //배경이미지 간격
        public MinMaxValue<int> ForegroundInterval = new MinMaxValue<int>(); //전경이미지 간격
        public MinMaxValue<int> FloorInterval = new MinMaxValue<int>(); //바닥이미지 간격


        [Header("배경 최대 수")]
        public MinMaxValue<int> ForegroundCount = new MinMaxValue<int>(); //전경이미지 최대 갯수

        [Header("배경이미지 소환 위치")]
        public Vector3 BackgroundPos = new Vector3(); //배경이미지 소환 위치
        public Vector3 ForegroundPos = new Vector3(); //전경이미지 소환 위치
        public Vector3 FloorPos = new Vector3(); //전경이미지 소환 위치

        [Header("Instantiate 부모 위치")]
        public GameObject BackParent;
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
            Game_Setting();
        }

        public void ScreenSizeCalcul() //임시 최대 경계선 계산
        {
            float yScreenHalfSize = Camera.main.orthographicSize;
            float xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;
            LeftPosX = -(xScreenHalfSize);
        }


        public void Game_Setting()
        {
            var SpawnPos = ForegroundPos;
            for (int i = 0; i < ForegroundObject.Count; i++) // 하나만 넣으면 한개만 반복 , 여러개 넣으면 순차적
            {
                SpriteRenderer spriteRenderer = ForegroundObject[i].GetComponent<SpriteRenderer>();
                ForegroundRender.Add(spriteRenderer);
                Vector2 pixelSize = spriteRenderer.bounds.size;
                ForegroundSize.Add(pixelSize);
                var obj = ForegroundObject[i];
                obj.runningGame = runningGame;
                PhoneUtil.SetLayer(obj);
                ForeFogSetting(obj, i);
            }
            SpriteRenderer BackSprite = BackgroundObject.GetComponent<SpriteRenderer>();
            Vector2 pisize = BackSprite.bounds.size;
            BackgroundSize = pisize;
            BackgroundObject.Setting(BackgroundSpeed);
            BackgroundObject.runningGame = runningGame;
            PhoneUtil.SetLayer(BackgroundObject);

            SpriteRenderer FloorSprite = FloorPrefab.GetComponent<SpriteRenderer>();
            pisize = FloorSprite.bounds.size *4; //4개 연결
            FloorSize = pisize;
            FloorObject.Setting(FloorSpeed);
            FloorObject.runningGame =runningGame;
            PhoneUtil.SetLayer(FloorObject);
        }

        public void ForeFogSetting(GroundObject target , int index)
        {
            SpriteRenderer Renderer = ForegroundRender[index];
            int randomValue = UnityEngine.Random.Range(0, 3); // 0 가깝고 흐릿함 / 1 중간 선명 //2 느리고 어두움  
            Vector3 Pos = target.transform.position;
            float RandomScale = UnityEngine.Random.Range(0.5f, 1.5f);
            target.transform.localScale = new Vector3(RandomScale, RandomScale,1);
            if (randomValue == 0)
            {
                Pos.z = 1f;
                target.Setting(UnityEngine.Random.Range(CloseFogSpeed.Min, CloseFogSpeed.Max));
            }
            else if (randomValue == 1)
            {
                Pos.z = 1.1f;
                target.Setting(UnityEngine.Random.Range(MiddleFogSpeed.Min, MiddleFogSpeed.Max));
            }
            else if (randomValue == 2)
            {
                Pos.z = 1.2f;
                target.Setting(UnityEngine.Random.Range(FarFogSpeed.Min, FarFogSpeed.Max));
            }
            Color pre = Renderer.color;
            if (randomValue == 0)
            {
                Renderer.color = new Color(1f, 1f, 1f, 150 / 255f);
            }
            if (randomValue == 1)
            {
                Renderer.color = new Color(1f, 1f, 1f, 1f);
            }
            if (randomValue == 2)
            {
                Renderer.color = new Color(240 / 255f, 240 / 255f, 240 / 255f, 1f);
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
            if (BackgroundObject.transform.position.x < LeftPosX + BackgroundSize.x / 4)
            {
                Vector3 pre = BackgroundObject.transform.position;
                pre.x += BackgroundSize.x / 4 + BackgroundInterval;
                BackgroundObject.transform.position = pre;
            }

            if (FloorObject.transform.position.x < LeftPosX + FloorSize.x/ 4)
            {
                Vector3 pre = FloorObject.transform.position;
                pre.x += FloorSize.x / 4;
                FloorObject.transform.position = pre;
            }

            for (int i = 0; i < ForegroundObject.Count; i++)
            {
                
                if (ForegroundObject[i].transform.position.x < LeftPosX - ForegroundSize[i].x / 2 * ForegroundObject[i].transform.localScale.x)
                {
                    ForeFogSetting(ForegroundObject[i] , i );
                    ForegroundObject[i].transform.position += Vector3.left * LeftPosX*4 * ForegroundObject[i].transform.localScale.x;
                }
            }
        }
    }
}