using DG.Tweening;
using System;
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
        public GroundObject BackgroundPrefab; // 생성할 배경 프리팹
        public List<GroundObject> ForegroundPrefab = new List<GroundObject>(); // 생성할 전경 프리팹
        public GroundObject FloorPrefab; // 생성할 바닥 프리팹

        [Header("생성된 배경")]
        public GroundObject backgroundObject; // 생성된 배경 이미지
        public SpriteRenderer backgroundSpriteRenderer; // 아예 아무런 배경이 없으면 뒤에가 빛춰 보임으로 prev 스프라이트를 넣어준다.
        public SpriteRenderer prevBackgroundSpriteRenderer;
        public MinMaxValue<float> updateBackgroundSpriteTimer = new(0,0,150, false, true); // 이미지가 몇초마다 바뀔 것인지
        public List<Sprite> backgroundSpriteList; // 배경에 사용될 이미지들
        private int backgroundSpriteIndex = 0;
        
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

        [Header("전체/해당 카운트 만큼 구름 생성")]
        public int backForegroundCount = 1; // (전체갯수/카운트)마다 생성되겠끔 함으로 카운트 수만큼 보장하진 않음. 


        [Header("전경 구름 속도")]
        public MinMaxValue<int> CloseFogSpeed = new MinMaxValue<int>(); 
        public MinMaxValue<int> MiddleFogSpeed = new MinMaxValue<int>(); 
        public MinMaxValue<int> FarFogSpeed = new MinMaxValue<int>(); 

        [Header("배경 간격")]
        public MinMaxValue<int> BackgroundInterval = new MinMaxValue<int>(); //배경이미지 간격
        public MinMaxValue<int> ForegroundInterval = new MinMaxValue<int>(); //전경이미지 간격
        public MinMaxValue<int> FloorInterval = new MinMaxValue<int>(); //바닥이미지 간격



        [Header("배경이미지 소환 위치")]
        public Vector3 BackgroundPos = new Vector3(); //배경이미지 소환 위치
        public Vector3 ForegroundPos = new Vector3(); //전경이미지 소환 위치
        public Vector3 FloorPos = new Vector3(); //전경이미지 소환 위치

        [Header("Instantiate 부모 위치")]
        public GameObject BackParent;
        public GameObject ForeParent;
        public GameObject FloorParent;

        public void Awake()
        {
            runningGame.inGame.onGameChangeEvent.AddListener(() =>
            {
                if (backgroundSpriteList.Count - 1 > backgroundSpriteIndex)
                {
                    backgroundSpriteRenderer.sprite = backgroundSpriteList[backgroundSpriteIndex];
                    prevBackgroundSpriteRenderer.sprite = backgroundSpriteList[backgroundSpriteIndex];
                    var pC = prevBackgroundSpriteRenderer.color;
                    pC.a = 1;
                    prevBackgroundSpriteRenderer.color = pC;
                    prevBackgroundSpriteRenderer.DOFade(0, 5f);
                    backgroundObject.spriteRenderer.sprite = backgroundSpriteList[++backgroundSpriteIndex];
                    var cC = backgroundObject.spriteRenderer.color;
                    cC.a = 0;
                    backgroundObject.spriteRenderer.color = cC;
                    backgroundObject.spriteRenderer.DOFade(1, 5f);
                    updateBackgroundSpriteTimer.Current -= updateBackgroundSpriteTimer.Max;
                    RenewalForeFogColor();
                }
            });
        }

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
                
                if (backForegroundCount>0&& i % backForegroundCount == 0)
                    ForegroundObject[i].groundType = 0;
                else
                    ForegroundObject[i].groundType = 1;

                SpriteRenderer spriteRenderer = ForegroundObject[i].GetComponent<SpriteRenderer>();
                ForegroundRender.Add(spriteRenderer);
                Vector2 pixelSize = spriteRenderer.bounds.size;
                ForegroundSize.Add(pixelSize);
                var obj = ForegroundObject[i];
                obj.runningGame = runningGame;
                PhoneUtil.SetLayer(obj);
                ForeFogSetting(obj, i);
            }
            SpriteRenderer BackSprite = backgroundObject.spriteRenderer.GetComponent<SpriteRenderer>();
            Vector2 pisize = BackSprite.bounds.size;
            BackgroundSize = pisize;
            backgroundObject.Setting(BackgroundSpeed);
            backgroundObject.runningGame = runningGame;
            PhoneUtil.SetLayer(backgroundObject);

            SpriteRenderer FloorSprite = FloorPrefab.GetComponent<SpriteRenderer>();
            pisize = FloorSprite.bounds.size *4; //4개 연결
            FloorSize = pisize;
            FloorObject.Setting(FloorSpeed);
            FloorObject.runningGame =runningGame;
            PhoneUtil.SetLayer(FloorObject);
        }

        public void ForeFogSetting(GroundObject target, int index) // isBack == 1 맨 뒤의 흐린 구름 전용
        {
            SpriteRenderer Renderer = ForegroundRender[index];
            Vector3 Pos = target.transform.position;
            float RandomScale = UnityEngine.Random.Range(1.5f, 2f);
            target.transform.localScale = new Vector3(RandomScale, RandomScale, 1);
            int randomValue = UnityEngine.Random.Range(1, 3); // 0 가깝고 흐릿함 / 1 중간 선명 //2 느리고 어두움  

            if (target.groundType == 0)
            {
                Pos.z = 1.2f;
                ForeFogSetColor(target, index, randomValue);
                target.Setting(UnityEngine.Random.Range(FarFogSpeed.Min, FarFogSpeed.Max));
            }
            else if (randomValue == 1)
            {
                Pos.z = 1f;
                target.Setting(UnityEngine.Random.Range(CloseFogSpeed.Min, CloseFogSpeed.Max));
                ForeFogSetColor(target, index, randomValue);
            }
            else if (randomValue == 2)
            {
                Pos.z = 1.1f;
                ForeFogSetColor(target, index, randomValue);
                target.Setting(UnityEngine.Random.Range(MiddleFogSpeed.Min, MiddleFogSpeed.Max));
            }
            target.transform.position = Pos;
        }

        public void ForeFogSetColor(GroundObject targets ,int targetIndexs , int posNum) // 구름 색깔 변경
        {

            SpriteRenderer Renderer = ForegroundRender[targetIndexs];
            if (targets.groundType == 0)
            {
                if (backgroundSpriteIndex == 0)
                {
                    Renderer.color = new Color(1f, 1f, 1f, 150 / 255f);
                }
                else if (backgroundSpriteIndex == 1)
                {
                    Renderer.color = new Color(1f, 200f / 255f, 120 / 255f, 1f);
                }
                else if (backgroundSpriteIndex == 2)
                {
                    Renderer.color = new Color(120 / 255f, 120f / 255f, 210 / 255f, 1f);
                }
            }
            else if (posNum == 1)
            {
                if (backgroundSpriteIndex == 0)
                {
                    Renderer.color = new Color(240 / 255f, 240 / 255f, 240 / 255f, 1f);
                }
                else if (backgroundSpriteIndex == 1)
                {
                    Renderer.color = new Color(200/255f, 150f / 255f, 100 / 255f, 1f);
                }
                else if (backgroundSpriteIndex == 2)
                {
                    Renderer.color = new Color(60 / 255f, 50f / 255f, 135 / 255f, 1f);

                }
            }
            else if (posNum == 2)
            {
                if (backgroundSpriteIndex == 0)
                {
                    Renderer.color = new Color(1f, 1f, 1f, 1f);
                }
                else if (backgroundSpriteIndex == 1)
                {
                    Renderer.color = new Color(200 / 255f, 150f / 255f, 150 / 255f, 1f);
                }
                else if (backgroundSpriteIndex == 2)
                {
                    Renderer.color = new Color(90 / 255f, 90f / 255f, 180 / 255f, 1f);

                }
            }
        }

        public void RenewalForeFogColor()
        {
            for (int i = 0; i < ForegroundObject.Count; i++)
            {

                if (ForegroundObject[i].transform.position.x > -LeftPosX + ForegroundSize[i].x / 2 * ForegroundObject[i].transform.localScale.x)
                {
                    int randomValue = UnityEngine.Random.Range(1, 3); // 0 가깝고 흐릿함 / 1 중간 선명 //2 느리고 어두움  
                    ForeFogSetColor(ForegroundObject[i], i, randomValue);
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
            if (backgroundObject.transform.position.x + BackgroundSize.x/2 < -LeftPosX)
            {
                Vector3 pre = backgroundObject.transform.position;
                float offset1 = BackgroundSize.x / 2 + LeftPosX * 2; //이미지 사이클의 시작부터 움직인 거리
                float offset2 = (backgroundObject.transform.position.x + BackgroundSize.x / 2 + LeftPosX); // 조건문으로 인한 거리 차이
                pre.x = LeftPosX + BackgroundSize.x/2 - offset1 + offset2;
                backgroundObject.transform.position = pre;
            }

            if (FloorObject.transform.position.x + FloorSize.x/2 < -LeftPosX) //화면 끝에 도달했을때
            {
                Vector3 pre = FloorObject.transform.position;
                float offset1 = FloorSize.x * 3 / 4 + LeftPosX * 2; // 이미지 사이클의 시작부터 움직인 거리 
                float offset2 = (FloorObject.transform.position.x + FloorSize.x / 2 + LeftPosX); // 조건문으로 인한 거리 차이
                pre.x = LeftPosX+ FloorSize.x/2 - offset1 + offset2; // 오차  계산
                FloorObject.transform.position = pre;
            }

            for (int i = 0; i < ForegroundObject.Count; i++)
            {
                
                if (ForegroundObject[i].transform.position.x < LeftPosX - ForegroundSize[i].x/2 * ForegroundObject[i].transform.localScale.x)
                {
                    ForeFogSetting(ForegroundObject[i] , i);
                    ForegroundObject[i].transform.position += Vector3.left * LeftPosX*4 * ForegroundObject[i].transform.localScale.x;
                }
            }
        }
    }
}