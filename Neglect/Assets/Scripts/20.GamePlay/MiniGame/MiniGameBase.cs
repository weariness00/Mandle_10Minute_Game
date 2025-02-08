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

        public virtual void Start()
        {
            
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
            if (renderTexture)
            {
                renderTexture.Release();
                Destroy(renderTexture);
                renderTexture = null;
            }
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
                // 미니 게임 씬에 있는 모든 객체는 Phone 레이어를 가지도록 변경
                foreach (Transform t in rootGameObject.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Phone");
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

        public void SetGameSpeed(float value)
        {
            gameSpeed.Value = value;
        }
    }
}

