using GamePlay.Phone;
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
        private static readonly string RunningGameScene = "Running Game";
        private static readonly string FlappingGameScene = "Flapping Game";

        public static void AddPhone() => SceneManager.LoadScene(PhoneScene, LoadSceneMode.Additive);
        public static void AsyncAddPhone(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(PhoneScene, loadedAction));

        public static Scene GetRunningGameScene() => SceneManager.GetSceneByName(RunningGameScene);
        public static bool TryGetRunningGameScene(out Scene scene)
        {
            scene = SceneManager.GetSceneByName(RunningGameScene);
            return scene.IsValid() && scene.isLoaded;
        }
        public static void AsyncAddRunningGame(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(RunningGameScene, loadedAction));

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
            AppLoad(scene);
            loadedAction?.Invoke(scene);
        }

        private void AppLoad(Scene scene)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                var app = rootGameObject.GetComponentInChildren<IPhoneApplication>();
                if (app != null)
                {
                    app.OnLoad();
                    break;
                }
            }
        }

        private void UnloadedObject(Scene scene)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                // Unloaded Scene 레이어에 해당하는 오브젝트 비활성화
                if (rootGameObject.layer == LayerMask.NameToLayer("Unloaded Scene"))
                {
                    rootGameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}

