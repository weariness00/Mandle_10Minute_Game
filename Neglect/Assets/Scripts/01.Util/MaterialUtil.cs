using AllIn1SpriteShader;
using System;
using UnityEngine;

namespace Util
{
    public class MaterialUtil : MonoBehaviour
    {
        public Renderer renderer;
        public MaterialPropertyBlock materialPropertyBlock;

        private bool isInit = false;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        public void Awake()
        {
            Init();
        }

        public void OnDestroy()
        {
            materialPropertyBlock.Clear();
        }

        public void Init()
        {
            if (isInit) return;
            isInit = true;
            materialPropertyBlock = new();
            renderer.GetPropertyBlock(materialPropertyBlock);
        }

        public void ApplyMaterial()
        {
            var texture = renderer.sharedMaterial.GetTexture(MainTex);
            if(!ReferenceEquals(texture, null))
                    materialPropertyBlock.SetTexture(MainTex, renderer.sharedMaterial.GetTexture(MainTex));
            renderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetFloat(string propertyName, float value)
        {
            Init();
            materialPropertyBlock.SetFloat(propertyName, value);
        }
        
        public void SetInt(string propertyName, int value)
        {
            Init();
            materialPropertyBlock.SetInt(propertyName, value);
        }

        public void SetKeyword(string propertyName, bool value)
        {
            if(value) renderer.material.EnableKeyword(propertyName);
            else renderer.material.DisableKeyword(propertyName);
        }
    }
}

