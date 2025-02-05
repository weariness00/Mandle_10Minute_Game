using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GamePlay
{
    public class SceneUtil : Singleton<SceneUtil>
    {
        private static readonly string PhoneScene = "Phone";
        private static readonly string FlappingGameScene = "Flapping Game";

        public static void AddPhone() => SceneManager.LoadScene(PhoneScene, LoadSceneMode.Additive);
        public static void AsyncAddPhone(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(PhoneScene, loadedAction));

        public static bool TryGetFlappingScene(out Scene scene)
        {
            scene = SceneManager.GetSceneByName(FlappingGameScene);
            return scene.IsValid() && scene.isLoaded;
        }
        public static void AddFlappingGame() => SceneManager.LoadScene(FlappingGameScene, LoadSceneMode.Additive);
        public static void AsyncAddFlappingGame(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(FlappingGameScene, loadedAction));

        private IEnumerator LoadSceneAsyncEnumerator(string sceneName, Action<Scene> loadedAction)
        {
            AsyncOperation asyncOperator = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncOperator.isDone)
            {
                yield return null;
            }
            var scene = SceneManager.GetSceneByName(sceneName);
            UnloadedObject(scene);
            loadedAction?.Invoke(scene);
        }

        private void UnloadedObject(Scene scene)
        {
            // 씬 로드 되면 일단 미니게임 초기화 하고 멈추기
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                // Additional Scene 레이어에 해당하는 오브젝트 비활성화
                if (rootGameObject.layer == LayerMask.NameToLayer("Additional Scene"))
                {
                    rootGameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}

