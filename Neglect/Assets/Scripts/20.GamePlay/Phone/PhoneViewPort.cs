using GamePlay.Phone;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Phone
{
    public partial class PhoneViewPort : MonoBehaviour
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
            
            vertical.Transform.SetParent(transform);
            horizon.Transform.SetParent(transform);
            horizon.Transform.localEulerAngles = new Vector3(0, 0, -90);
        }

        public void SetShader(Shader shader)
        {
            vertical.SetShader(shader);
            horizon.SetShader(shader);
        }

        public void SetActive(PhoneViewType type)
        {
            switch (type)
            {
                case PhoneViewType.Vertical:
                    vertical.GameObject.SetActive(true);
                    horizon.GameObject.SetActive(false);
                    break;
                case PhoneViewType.Horizon:
                    vertical.GameObject.SetActive(false);
                    horizon.GameObject.SetActive(true);
                    break;
                default: return;
            }
        }
        
        public void Release()
        {
            vertical.Release();
            horizon.Release();
        }
    }

    public partial class PhoneViewPort
    {
        [Serializable]
        public class RenderTextureData
        {
            public SpriteRenderer spriteRenderer;
            public RawImage renderTextureImage;
            public RenderTexture renderTexture;

            private static readonly int RenderTexture = Shader.PropertyToID("_Render_Texture");

            public GameObject GameObject => spriteRenderer.gameObject;
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
    }
}