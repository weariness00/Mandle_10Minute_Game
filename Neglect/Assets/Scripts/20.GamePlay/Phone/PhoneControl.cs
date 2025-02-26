using DG.Tweening;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GamePlay.Phone
{
    public partial class PhoneControl : MonoBehaviour
    {
        [Header("Phone 관련")] public string phoneName = "None";
        public Camera phoneCamera;
        public Vector2Int phoneVerticalViewPortSize = new Vector2Int(600, 960);
        public Vector2Int phoneHorizonViewPortSize => new Vector2Int(phoneVerticalViewPortSize.y, phoneVerticalViewPortSize.x);

        public SpriteRenderer fadeRenderer;
        public GameObject ChargingPort;
        public PhoneSideButton interfaceGroupOnOffButton;

        private Tween fadeTween; // Fade In&Out 관련 Tween
        
        [Header("App 관련")] 
        public ApplicationControl applicationControl;
        public void Awake()
        {
            { // 테스틑용 
                PhoneUtil.Release();
                PhoneUtil.currentPhone = this;
            }
            PhoneUtil.AddPhone(this);

            InteractInit();

            applicationControl.OnAddAppEvent.AddListener(app =>
            {
                var obj = (app as MonoBehaviour)?.gameObject;
                foreach (GameObject rootGameObject in obj.scene.GetRootGameObjects())
                {
                    // 미니 게임 씬에 있는 모든 객체는 Phone 레이어를 가지도록 변경
                    foreach (Transform t in rootGameObject.GetComponentsInChildren<Transform>(true))
                    {
                        t.gameObject.layer = LayerMask.NameToLayer("Phone");
                    }
                }

                // 폰에 app에 따른 Render Texture 할당
                var phoneViewPort = Instantiate(phoneViewPortPrefab, transform);
                phoneViewPort.MakeTextureObject(app.VerticalResolution);
                phoneViewPort.vertical.spriteRenderer.sprite = phoneVerticalSprite;
                phoneViewPort.horizon.spriteRenderer.sprite = phoneHorizonSprite; // 세로는 다른 이미지 사용해야된다.
                phoneViewPort.SetShader(phoneShader);
                phoneViewPort.gameObject.SetActive(false);
                phoneViewPort.name = app.AppName;
                phoneViewPortDictionary.Add(app.AppName, phoneViewPort);
            });
            
            applicationControl.OnAppEvent.AddListener(app =>
            {
                var viewPort = phoneViewPortDictionary[app.AppName];
                ChangeViewPort(viewPort);
            });
            
            applicationControl.OnAppResumeEvent.AddListener(app =>
            {
                var viewPort = phoneViewPortDictionary[app.AppName];
                ChangeViewPort(viewPort);
            });

            // 폰 카메라 생성 & 셋팅
            if (phoneCamera == null)
            {
                phoneCamera = Instantiate(Camera.main);
                phoneCamera.name = "Phone Camera";
                phoneCamera.cullingMask = LayerMask.GetMask("Phone");
                var uac = phoneCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                uac.renderPostProcessing = false;
                Destroy(phoneCamera.GetComponent<AudioListener>());
            }
        }

        public void Start()
        {
            SceneUtil.AsyncAddHome(scene =>
            {
                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                {
                    var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                    if (app != null)
                    {
                        applicationControl.AddApp(app);
                        applicationControl.OpenApp(app);
                    }
                }
            });
        }

        public void Update()
        {
            UIInteract();
        }

        public void OnDestroy()
        {
            foreach (var phoneViewPort in phoneViewPortDictionary.Values)
                phoneViewPort.Release();
        }
    }
    
    // 기타 효과
    public partial class PhoneControl
    {
        [Header("기타 효과 관련")] 
        [Tooltip("화면에 상호작용이 안되는 빈 공간을 눌렀을때")] public AudioClip emptyClickSound;
        [Tooltip("핸드폰 진동 사운드")]public AudioClip vibrationSound;
        
        public void FadeOut(float duration, Color color, float delay = 0f)
        {
            fadeTween?.Kill();

            var a = fadeRenderer.color.a;
            color.a = a;
            fadeRenderer.color = color;
            fadeTween = fadeRenderer.DOFade(1f, duration).SetDelay(delay);
        }
        
        public void FadeIn(float duration, Color color, float delay = 0f)
        {
            fadeTween?.Kill();
            
            var a = fadeRenderer.color.a;
            color.a = a;
            fadeRenderer.color = color;
            fadeTween = fadeRenderer.DOFade(0f, duration).SetDelay(delay);
        }
        
        // 핸드폰 진동
        public void PhoneVibration(float duration = 0.1f)
        {
            transform.DOShakePosition(duration, 0.3f, 50, 90f);
            SoundManager.Instance.GetAudioSource("Effect").PlayOneShot(vibrationSound);
        }
    }
        

    // 렌더 텍스쳐
    public partial class PhoneControl
    {
        [Header("Phone View Port 관련")]
        public Shader phoneShader;
        public Sprite phoneVerticalSprite;
        public Sprite phoneHorizonSprite;
        
        private Dictionary<string, PhoneViewPort> phoneViewPortDictionary = new();
        [Tooltip("생성될 Phone View Port 프리펩")]public PhoneViewPort phoneViewPortPrefab;
        public PhoneViewPort currentPhoneViewPort;
        public PhoneViewType viewType;

        public PhoneViewPort GetAppViewPort(IPhoneApplication app) => phoneViewPortDictionary.GetValueOrDefault(app.AppName);

        public void PhoneViewRotate(int value) => PhoneViewRotate(value, null);
        public void PhoneViewRotate(PhoneViewType value) => PhoneViewRotate((int)value);
        public void PhoneViewRotate(PhoneViewType value, Action isRotated) => PhoneViewRotate((int)value, isRotated);
        public void PhoneViewRotate(int value, Action isRotated)
        {
            viewType = (PhoneViewType)value;

            var sequence = DOTween.Sequence();
            switch (viewType)
            {
                case PhoneViewType.Vertical:
                    sequence.Append(transform.DORotate(new Vector3(0, 0, 0), 1f).OnComplete(() =>
                    {
                        currentPhoneViewPort.horizon.SetActive(false);
                        currentPhoneViewPort.vertical.SetActive(true);
                        phoneCamera.targetTexture = currentPhoneViewPort.vertical.renderTexture;
                        isRotated?.Invoke();
                    }));
                    break;
                case PhoneViewType.Horizon:
                    sequence.Append(transform.DORotate(new Vector3(0, 0, 90), 1f).OnComplete(() =>
                    {
                        currentPhoneViewPort.vertical.SetActive(false);
                        currentPhoneViewPort.horizon.SetActive(true);
                        phoneCamera.targetTexture = currentPhoneViewPort.horizon.renderTexture;
                        isRotated?.Invoke();
                    }));
                    break;
            }
        }

        public void ChangeViewPort(PhoneViewPort viewPort)
        {
            var prevPhoneViewPort = currentPhoneViewPort;
            if (prevPhoneViewPort != viewPort)
            {
                if (prevPhoneViewPort != null)
                {
                    var currentPos = currentPhoneViewPort.transform.position;
                    currentPos.z = 10;
                    currentPhoneViewPort.transform.position = currentPos;
                }
                // 각종 연출 효과를 위해 0.3초 뒤에 이전 ViewPort를 비활성화
                Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
                {
                    if (prevPhoneViewPort != null)
                        prevPhoneViewPort.gameObject.SetActive(false);
                });
            }

            viewPort.gameObject.SetActive(true);
            viewPort.SetActive(viewType);
            phoneCamera.targetTexture = viewPort.GetData(viewType).renderTexture;
            currentPhoneViewPort = viewPort;
            isUpdateInteract = true;
        }
    }

    public partial class PhoneControl
    {
        [Header("Phone View 상호작용 관련")]
        private PointerEventData pointerData;
        private List<RaycastResult> rayCastResults = new List<RaycastResult>();
        private GameObject lastHoveredObject = null; // 마지막으로 마우스를 올린 UI
        private GameObject lastPressedObject = null; // 마지막으로 클릭한 UI
        private GameObject draggingObject = null;
        public bool isUpdateInteract = false;
        public bool isInMousePosition = true;

        public Vector2 phoneMousePosition;
        private Vector2 prevMousePosition;
        
        private void InteractInit()
        {
            pointerData = new PointerEventData(EventSystem.current);
        }

        // Phone과 Object들의 마우스 상호작용 용으로 사용할때
        public Vector3 ScreenToWorldPoint()
        {
            return !isUpdateInteract ? Vector2.positiveInfinity : phoneCamera.ScreenToWorldPoint(phoneMousePosition);
        }

        // UI와 상호작용할때
        public void UIInteract()
        {
            if(isUpdateInteract == false) return;
            var curRenderData = currentPhoneViewPort.GetData(viewType); // 임시용
            float unitToPixel = Screen.height / (Camera.main.orthographicSize * 2); // 1Unit 당 몇 픽셀인지
            Vector2 screenSize = new Vector2Int(Screen.width, Screen.height); // 현재 해상도 크기
            Vector2 renderTextureSize = new(curRenderData.renderTexture.width, curRenderData.renderTexture.height); // 렌더 텍스쳐의 크기
            Vector2 spriteTextureSize = (curRenderData.spriteRenderer.sprite.rect.size / curRenderData.spriteRenderer.sprite.pixelsPerUnit) * curRenderData.spriteRenderer.transform.lossyScale * unitToPixel; // 스프라이트의 픽셀 크기 * 오브젝트 scale
            Vector2 viewRatio = renderTextureSize / spriteTextureSize; // 렌더 텍스쳐 해상도 : 스프라이트 해상도
            Vector2 mousePosition = Mouse.current.position.ReadValue(); // 마우스의 현재 위치
            Vector2 objPosition = curRenderData.spriteRenderer.transform.position * unitToPixel; // 오브젝트의 현재 위치를 viewport 위치로 전환
            // Vector2 cameraPosition = phoneCamera.transform.position * unitToPixel; // 카메라의 현재 위치를 viewport 위치로 전환
            phoneMousePosition = (mousePosition - objPosition - screenSize / 2) * viewRatio + renderTextureSize / 2;
            isInMousePosition = false;
            if (phoneMousePosition.x < 0 || phoneMousePosition.x > renderTextureSize.x ||
                phoneMousePosition.y < 0 || phoneMousePosition.y > renderTextureSize.y)
            {
                if (!ReferenceEquals(lastHoveredObject, null))
                {
                    ExecuteEvents.Execute(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
                    lastHoveredObject = null;
                }

                if (!ReferenceEquals(lastPressedObject, null))
                {
                    ExecuteEvents.ExecuteHierarchy(lastPressedObject, pointerData, ExecuteEvents.pointerUpHandler);
                    lastPressedObject = null;
                }
                if (!ReferenceEquals(draggingObject, null))
                {
                    ExecuteEvents.ExecuteHierarchy(draggingObject, pointerData, ExecuteEvents.endDragHandler); // 드래그 종료
                    ExecuteEvents.ExecuteHierarchy(draggingObject, pointerData, ExecuteEvents.dropHandler); // 드래그 종료
                    draggingObject = null;
                }
                return;
            }

            isInMousePosition = true;
            pointerData.position = phoneMousePosition;
            pointerData.delta = phoneMousePosition - prevMousePosition;
            prevMousePosition = phoneMousePosition;
            rayCastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, rayCastResults);

            var phoneObject = rayCastResults.Where(r => r.gameObject.layer == LayerMask.NameToLayer("Phone")).ToArray();
            if (phoneObject.Length <= 0)
            {
                if (!ReferenceEquals(lastHoveredObject, null))
                {
                    // Exit 이벤트 (마우스가 UI에서 벗어날 때)
                    ExecuteEvents.Execute(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
                    lastHoveredObject = null;
                }
                return;
            }

            pointerData.pointerCurrentRaycast = phoneObject[0];
            pointerData.pointerPressRaycast = phoneObject[0];
            var hitUI = phoneObject[0].gameObject;
            // 🖱️ 마우스가 UI 위에 있는 경우 (Hover)
            // Enter 이벤트 (마우스가 새로 UI에 올라갔을 때)
            var hoveredObj = ExecuteEvents.GetEventHandler<IPointerEnterHandler>(hitUI);
            if (!ReferenceEquals(hoveredObj, lastHoveredObject))
            {
                ExecuteEvents.Execute(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
                lastHoveredObject = ExecuteEvents.ExecuteHierarchy(hoveredObj, pointerData, ExecuteEvents.pointerEnterHandler);
            }
            
            // 🖱️ 마우스 버튼이 눌렸을 때 (Click Down)
            if (
#if ENABLE_INPUT_SYSTEM
                Mouse.current.leftButton.wasPressedThisFrame
#else
                Input.GetMouseButtonDown(0)
#endif
            )
            {
                draggingObject = ExecuteEvents.ExecuteHierarchy(hitUI, pointerData, ExecuteEvents.initializePotentialDrag);
                pointerData.pointerPress = draggingObject;
                
                var downObj = ExecuteEvents.ExecuteHierarchy(hitUI, pointerData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(downObj, pointerData, ExecuteEvents.selectHandler); // 선택 처리
                lastPressedObject = downObj;
                pointerData.pointerPress = downObj;

                var isDragNull = ReferenceEquals(draggingObject, null);
                var isLastPressNull = ReferenceEquals(lastPressedObject, null);
                if (!isDragNull)
                {
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.beginDragHandler);
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.selectHandler); // 선택 처리
                }
                // 아무것도 클릭 된게 없으면
                else if (isLastPressNull)
                {
                    SoundManager.Instance.GetAudioSource("Effect").PlayOneShot(emptyClickSound);
                }
            }

            // 🖱️ 마우스 버튼을 떼었을 때 (Click Up)
            if (
#if ENABLE_INPUT_SYSTEM
                Mouse.current.leftButton.wasReleasedThisFrame
#else
                Input.GetMouseButtonUp(0)
#endif
               )
            {
                var upObj = ExecuteEvents.ExecuteHierarchy(hitUI, pointerData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(lastPressedObject, pointerData, ExecuteEvents.pointerUpHandler);

                // 클릭이 같은 오브젝트에서 발생한 경우 클릭 이벤트 실행
                if (ReferenceEquals(upObj, lastPressedObject))
                    ExecuteEvents.Execute(lastPressedObject, pointerData, ExecuteEvents.pointerClickHandler);

                lastPressedObject = null;
                pointerData.pointerPress = null;
            }

            // 🖱️ 마우스 이동 중일 때 (Drag)
            if (
#if ENABLE_INPUT_SYSTEM
                Mouse.current.leftButton.isPressed
#else
                Input.GetMouseButton(0)
#endif
            ) // 마우스가 눌린 상태에서
            { 
                if (!ReferenceEquals(draggingObject, null))
                {
                    pointerData.dragging = true;
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.dragHandler); // 드래그 중
                }
                if (!ReferenceEquals(lastPressedObject, null))
                {
                    ExecuteEvents.Execute(lastPressedObject, pointerData, ExecuteEvents.pointerMoveHandler); // 움직이는 중
                }
            }
            else
            {
                if (!ReferenceEquals(draggingObject, null))
                {
                    pointerData.dragging = false;
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.endDragHandler); // 드래그 종료
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.dropHandler); // 드래그 종료
                    draggingObject = null;
                }

                lastPressedObject = null;
            }
        }
    }
}