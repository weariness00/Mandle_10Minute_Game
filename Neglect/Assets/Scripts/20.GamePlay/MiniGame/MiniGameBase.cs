using Manager;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace GamePlay.MiniGame
{
    public class MiniGameBase : MonoBehaviour
    {
        public ReactiveProperty<bool> isGamePlay = new(true);
        public ReactiveProperty<float> gameSpeed = new(1f);
        [SerializeField] protected MinMaxValue<float> playTime = new(0,0, 60 * 10);

        private RawImage renderTextureImage;
        private RenderTexture renderTexture;

        public virtual void Awake()
        {
            playTime.SetMin();
        }
        
        public virtual void Update()
        {
            if (isGamePlay.Value)
            {
                playTime.Current += Time.deltaTime;
            }
        }

        public virtual void OnDestroy()
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }

        public virtual void GamePlay()
        {
            isGamePlay.Value = true;
        }

        public virtual void GameStop()
        {
            isGamePlay.Value = false;
        }

        public virtual void GameOver()
        {
            isGamePlay.Value = false;
            gameSpeed.Value = 0;
        }

        public void InitLoadedScene(Scene scene)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                var i = LayerMask.NameToLayer("Additional Scene");
                var i2 = LayerMask.GetMask("Additional Scene");
                var b = i == i2;
                if (rootGameObject.layer == i)
                {
                    rootGameObject.SetActive(false);
                    break;
                }
            }

            // 폰 카메라에 사용될 Render Texture 생성
            renderTexture = new RenderTexture(960, 600, 16);
            renderTexture.Create();

            renderTextureImage = UIManager.InstantiateRenderTextureImage(960, 600);
            renderTextureImage.rectTransform.anchorMin = Vector2.zero;
            renderTextureImage.rectTransform.anchorMax = Vector2.one;
            renderTextureImage.rectTransform.offsetMin = Vector2.zero;
            renderTextureImage.rectTransform.offsetMax = Vector2.zero;
            renderTextureImage.texture = renderTexture;
            
            // 폰 카메라 생성 & 셋팅
            var phoneCamera = Instantiate(Camera.main);
            phoneCamera.cullingMask = LayerMask.GetMask("Phone");
            phoneCamera.targetTexture = renderTexture;
            Destroy(phoneCamera.gameObject.GetComponent<AudioListener>());
        }
    }
}

