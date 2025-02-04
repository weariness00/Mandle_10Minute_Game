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
        private static readonly string FlappingGameScene = "Flapping Game";
        
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
            loadedAction?.Invoke(SceneManager.GetSceneByName(sceneName));
        }
    }
}

