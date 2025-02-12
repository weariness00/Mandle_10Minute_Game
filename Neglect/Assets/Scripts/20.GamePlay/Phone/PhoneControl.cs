using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GamePlay.Phone
{
    public partial class PhoneControl : MonoBehaviour
    {
        [Header("Phone 관련")] public string phoneName = "None";
        public Camera phoneCamera;
        public Canvas phoneCanvas;
        public Canvas phoneUICanvas;
        public Vector2Int phoneVerticalViewPortSize = new Vector2Int(600, 960);
        public Vector2Int phoneHorizonViewPortSize => new Vector2Int(phoneVerticalViewPortSize.y, phoneVerticalViewPortSize.x);

        [Header("App 관련")] 
        public ApplicationControl applicationControl;

        public void Awake()
        {
            InteractInit();

            applicationControl.OnAddAppEvent.AddListener(app =>
            {
                PhoneViewPort phoneViewPort = new();
                phoneViewPort.MakeTextureObject(app.VerticalResolution);
                phoneViewPort.SetParent(transform);
                phoneViewPort.vertical.spriteRenderer.sprite = phoneSprite;
                phoneViewPort.horizon.spriteRenderer.sprite = phoneSprite; // 세로는 다른 이미지 사용해야된다.
                phoneViewPort.SetShader(phoneShader);
                phoneViewPort.horizon.SetActive(false);
                phoneViewPortDictionary.Add(app.AppName, phoneViewPort);
                
                // 임시 코드들
                currentPhoneViewPort = phoneViewPort; 
                phoneCamera.targetTexture = phoneViewPortDictionary.FirstOrDefault().Value.vertical.renderTexture;
            });
            
            applicationControl.OnAppEvent.AddListener(app =>
            {
                isUpdateInteract = true;
            });
            
            PhoneUtil.phoneDictionary[phoneName] = this;

            // 폰 카메라 생성 & 셋팅
            phoneCamera = Instantiate(Camera.main);
            phoneCamera.cullingMask = LayerMask.GetMask("Phone");
            Destroy(phoneCamera.GetComponent<AudioListener>());
        }

        public void Update()
        {
            Interact();
        }

        public void OnDestroy()
        {
            foreach (var phoneViewPort in phoneViewPortDictionary.Values)
                phoneViewPort.Release();
        }
    }

    // 렌더 텍스쳐
    public partial class PhoneControl
    {
        [Header("Phone View Port 관련")]
        public Shader phoneShader;
        public Sprite phoneSprite;
        
        public Dictionary<string, PhoneViewPort> phoneViewPortDictionary = new();
        public PhoneViewPort currentPhoneViewPort;
        public PhoneViewType viewType;

        public class RenderTextureData
        {
            public SpriteRenderer spriteRenderer;
            public RawImage renderTextureImage;
            public RenderTexture renderTexture;

            private static readonly int RenderTexture = Shader.PropertyToID("_Render_Texture");

            public RectTransform RectTransform => renderTextureImage.rectTransform;
            public Transform Transform => spriteRenderer.transform;

            public void MakePhoneObjectTexture(Vector2Int size)
            {
                // 폰 카메라에 사용될 Render Texture 생성
                renderTexture = new RenderTexture(size.x, size.y, 16);
                renderTexture.Create();

                var obj = new GameObject("Render Texture Object");
                spriteRenderer = obj.AddComponent<SpriteRenderer>();
            }

            public void SetShader(Shader shader)
            {
                var material = new Material(shader);
                material.SetTexture(RenderTexture, renderTexture);
                if (spriteRenderer) spriteRenderer.SetMaterials(new() { material });
                if (renderTextureImage) renderTextureImage.material = material;
            }

            public void MakePhoneUITexture(Vector2Int size)
            {
                // 폰 카메라에 사용될 Render Texture 생성
                renderTexture = new RenderTexture(size.x, size.y, 16);
                renderTexture.Create();

                var imageObj = new GameObject("Render Texture Image");
                renderTextureImage = imageObj.AddComponent<RawImage>();
                renderTextureImage.texture = renderTexture;

                // renderTextureImage = UIManager.InstantiateRenderTextureImage(size.x, size.y);
                // renderTextureImage.texture = renderTexture;
            }

            public void SetActive(bool value)
            {
                if (renderTextureImage) renderTextureImage.gameObject.SetActive(value);
                if (spriteRenderer) spriteRenderer.gameObject.SetActive(value);
            }

            public void Release()
            {
                if (renderTexture)
                {
                    renderTexture.Release();
                    Destroy(renderTexture);
                    renderTexture = null;
                }
            }
        }

        [Serializable]
        public class PhoneViewPort
        {
            public RenderTextureData vertical = new();
            public RenderTextureData horizon = new();

            public RenderTextureData GetData(PhoneViewType type)
            {
                switch (type)
                {
                    case PhoneViewType.Vertical: return vertical;
                    case PhoneViewType.Horizon: return horizon;
                    default: return null;
                }
            }

            public void MakeTextureObject(Vector2Int verticalSize)
            {
                vertical.MakePhoneObjectTexture(verticalSize);
                horizon.MakePhoneObjectTexture(new Vector2Int(verticalSize.y, verticalSize.x));
                horizon.Transform.localEulerAngles = new Vector3(0, 0, -90);
            }

            public void SetShader(Shader shader)
            {
                vertical.SetShader(shader);
                horizon.SetShader(shader);
            }

            public void SetParent(Transform parent)
            {
                vertical.Transform.SetParent(parent);
                horizon.Transform.SetParent(parent);
            }

            public void Release()
            {
                vertical.Release();
                horizon.Release();
            }
        }

        public void PhoneViewRotate(int value)
        {
            viewType = (PhoneViewType)value;

            var sequence = DOTween.Sequence();
            switch (viewType)
            {
                case PhoneViewType.Vertical:
                    sequence.Append(transform.DORotate(new Vector3(0, 0, 0), 1f).OnComplete(() =>
                    {
                        currentPhoneViewPort.vertical.SetActive(true);
                        currentPhoneViewPort.horizon.SetActive(false);
                        phoneCamera.targetTexture = currentPhoneViewPort.vertical.renderTexture;

                        var phoneUIRect = phoneUICanvas.GetComponent<RectTransform>();
                        phoneUIRect.sizeDelta = phoneHorizonViewPortSize;
                        phoneUIRect.localEulerAngles = new Vector3(0, 0, 0);
                    }));
                    break;
                case PhoneViewType.Horizon:
                    sequence.Append(transform.DORotate(new Vector3(0, 0, 90), 1f).OnComplete(() =>
                    {
                        currentPhoneViewPort.vertical.SetActive(false);
                        currentPhoneViewPort.horizon.SetActive(true);
                        phoneCamera.targetTexture = currentPhoneViewPort.horizon.renderTexture;

                        var phoneUIRect = phoneUICanvas.GetComponent<RectTransform>();
                        phoneUIRect.sizeDelta = phoneHorizonViewPortSize;
                        phoneUIRect.localEulerAngles = new Vector3(0, 0, -90);
                    }));
                    break;
            }
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
        
        private void InteractInit()
        {
            pointerData = new PointerEventData(EventSystem.current);
        }

        public void Interact()
        {
            if(isUpdateInteract == false) return;
            var curRenderData = currentPhoneViewPort.GetData(viewType); // 임시용
            float unitToPixel = Screen.height / (Camera.main.orthographicSize * 2); // 1Unit 당 몇 픽셀인지
            Vector2 screenSize = new Vector2Int(Screen.width, Screen.height); // 현재 해상도 크기
            Vector2 renderTextureSize = new(curRenderData.renderTexture.width, curRenderData.renderTexture.height); // 렌더 텍스쳐의 크기
            // Vector2 spriteTextureSize = curPort.renderTextureImage.rectTransform.sizeDelta * curPort.renderTextureImage.transform.lossyScale; // 스프라이트의 픽셀 크기 * 오브젝트 scale
            Vector2 spriteTextureSize = (curRenderData.spriteRenderer.sprite.rect.size / curRenderData.spriteRenderer.sprite.pixelsPerUnit) * curRenderData.spriteRenderer.transform.lossyScale * unitToPixel; // 스프라이트의 픽셀 크기 * 오브젝트 scale
            Vector2 viewRatio = renderTextureSize / spriteTextureSize; // 렌더 텍스쳐 해상도 : 스프라이트 해상도
            Vector2 mousePosition = Mouse.current.position.ReadValue(); // 마우스의 현재 위치
            Vector2 objPosition = curRenderData.spriteRenderer.transform.position * unitToPixel; // 오브젝트의 현재 위치를 viewport 위치로 전환
            // Vector2 objPosition = transform.position * unitToPixel; // 오브젝트의 현재 위치를 viewport 위치로 전환
            Vector2 cameraPosition = phoneCamera.transform.position * unitToPixel; // 카메라의 현재 위치를 viewport 위치로 전환
            Vector2 phoneMousePosition = (mousePosition - objPosition + cameraPosition - screenSize / 2) * viewRatio + renderTextureSize / 2;

            isInMousePosition = false;
            if (phoneMousePosition.x < 0 || phoneMousePosition.x > renderTextureSize.x ||
                phoneMousePosition.y < 0 || phoneMousePosition.y > renderTextureSize.y)
                return;

            isInMousePosition = true;
            pointerData.position = phoneMousePosition;
            rayCastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, rayCastResults);

            Debug.Log(phoneMousePosition);

            var phoneObject = rayCastResults.Where(r => r.gameObject.layer == LayerMask.NameToLayer("Phone")).ToArray();
            if (phoneObject.Length <= 0)
            {
                if (lastHoveredObject != null)
                {
                    // Exit 이벤트 (마우스가 UI에서 벗어날 때)
                    ExecuteEvents.ExecuteHierarchy(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
                    lastHoveredObject = null;
                }

                return;
            }

            var hitUI = phoneObject[0].gameObject;
            // 🖱️ 마우스가 UI 위에 있는 경우 (Hover)
            // Enter 이벤트 (마우스가 새로 UI에 올라갔을 때)
            if (lastHoveredObject != hitUI)
            {
                if (lastHoveredObject != null)
                    ExecuteEvents.ExecuteHierarchy(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
                ExecuteEvents.ExecuteHierarchy(hitUI, pointerData, ExecuteEvents.pointerEnterHandler);
            }

            lastHoveredObject = hitUI;

            // 🖱️ 마우스 버튼이 눌렸을 때 (Click Down)
            if (
#if ENABLE_INPUT_SYSTEM
                Mouse.current.leftButton.wasReleasedThisFrame
#else
                Input.GetMouseButtonDown(0)
#endif
            )
            {
                pointerData.pointerPress = hitUI;
                lastPressedObject = hitUI;

                ExecuteEvents.ExecuteHierarchy(hitUI, pointerData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.ExecuteHierarchy(hitUI, pointerData, ExecuteEvents.selectHandler); // 선택 처리
            }

            // 🖱️ 마우스 버튼을 떼었을 때 (Click Up)
            if (
#if ENABLE_INPUT_SYSTEM
                Mouse.current.leftButton.wasReleasedThisFrame
#else
                Input.GetMouseButtonUp(0)
#endif
                && lastPressedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(lastPressedObject, pointerData, ExecuteEvents.pointerUpHandler);

                // 클릭이 같은 오브젝트에서 발생한 경우 클릭 이벤트 실행
                if (hitUI == lastPressedObject)
                    ExecuteEvents.ExecuteHierarchy(lastPressedObject, pointerData, ExecuteEvents.pointerClickHandler);

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
                if (lastPressedObject != null && draggingObject == null)
                {
                    draggingObject = lastPressedObject;
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.dragHandler); // 드래그 시작
                }
                else if (draggingObject != null)
                {
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.dragHandler); // 드래그 중
                }
            }
            else
            {
                if (draggingObject != null)
                {
                    ExecuteEvents.Execute(draggingObject, pointerData, ExecuteEvents.dropHandler); // 드래그 종료
                    draggingObject = null;
                }
            }
        }
    }
}