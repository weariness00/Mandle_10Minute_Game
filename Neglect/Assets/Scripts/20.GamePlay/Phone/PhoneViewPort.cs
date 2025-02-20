using GamePlay.Phone;
using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Phone
{
    public partial class PhoneViewPort : MonoBehaviour
    {
        public RenderTextureData vertical = new();
        public RenderTextureData horizon = new();

        [Header("각종 효과")]
        public MMF_Player enableMMFPlayer;

        public void OnEnable()
        {
            enableMMFPlayer.PlayFeedbacks();
            if(vertical.HasData()) vertical.Transform.localScale = Vector3.one;
            if(vertical.HasData()) horizon.Transform.localScale = Vector3.one;
        }

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

            vertical.GameObject.name = "Vertical";
            vertical.renderTexture.name ="Vertical";
            horizon.GameObject.name = "Horizon";
            horizon.renderTexture.name = "Horizon";
            
            vertical.Transform.SetParent(transform, false);
            horizon.Transform.SetParent(transform, false);
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
                    horizon.GameObject.SetActive(false);
                    vertical.GameObject.SetActive(true);
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
            public RenderTexture renderTexture;

            private static readonly int RenderTexture = Shader.PropertyToID("_Render_Texture");

            public GameObject GameObject => spriteRenderer.gameObject;
            public Transform Transform => spriteRenderer.transform;

            public bool HasData() => spriteRenderer != null;

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
            }

            public Material GetMaterial() => spriteRenderer.material;
            public void SetTexture(Texture texture) => spriteRenderer.material.SetTexture(RenderTexture, renderTexture);
            
            public void SetActive(bool value)
            {
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

                if (spriteRenderer)
                {
                    Destroy(spriteRenderer.gameObject);
                }
            }
        }
    }
}