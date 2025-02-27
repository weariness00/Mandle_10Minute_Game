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
        private static readonly string RealScene = "Real";
        private static readonly string PhoneScene = "Phone";
        private static readonly string HomeScene = "Home";
        private static readonly string DummyScene = "Dummy App";
        private static readonly string SettingScene = "Setting App";
        private static readonly string TutorialScene = "Game Tutorial App";
        private static readonly string RunningGameScene = "Running Game";
        private static readonly string FlappingGameScene = "Flapping Game";
        private static readonly string BankScene = "BankApp";
        private static readonly string ChattingScene = "Chatting App";
        private static readonly string GameResultScene = "Game Result App";

        public static void LoadReal(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneEnumerator(RealScene, loadedAction));
        
        public static bool TryGetPhoneScene(out Scene scene)
        {
            scene = SceneManager.GetSceneByName(PhoneScene);
            return scene.IsValid() && scene.isLoaded;
        }
        public static void AsyncAddPhone(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(PhoneScene, loadedAction));
        public static void AsyncAddHome(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(HomeScene, loadedAction));
        public static void AsyncAddDummy(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(DummyScene, loadedAction));
        public static void AsyncAddSetting(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(SettingScene, loadedAction));
        public static void AsyncAddTutorial(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(TutorialScene, loadedAction));
        public static void AsyncAddBank(Action<Scene> loadedAction = null) => Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(BankScene, loadedAction));
        public static void AsyncAddChatting(Action<Scene> loadedAction = null) => Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(ChattingScene, loadedAction));
        
        public static Scene GetRunningGameScene() => SceneManager.GetSceneByName(RunningGameScene);
        public static bool TryGetRunningGameScene(out Scene scene)
        {
            scene = SceneManager.GetSceneByName(RunningGameScene);
            return scene.IsValid() && scene.isLoaded;
        }
        
        
        public static void LoadRunningGame(Action<Scene> loadedAction = null)=> Instance.StartCoroutine(Instance.LoadSceneEnumerator(RunningGameScene, loadedAction));
        public static void AsyncAddRunningGame(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(RunningGameScene, loadedAction));

        public static bool TryGetFlappingScene(out Scene scene)
        {
            scene = SceneManager.GetSceneByName(FlappingGameScene);
            return scene.IsValid() && scene.isLoaded;
        }
        public static bool TryGetBankScene(out Scene scene)
        {
            scene = SceneManager.GetSceneByName(FlappingGameScene);
            return scene.IsValid() && scene.isLoaded;
        }
        public static void AddFlappingGame() => SceneManager.LoadScene(FlappingGameScene, LoadSceneMode.Additive);
        public static void AsyncAddFlappingGame(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(FlappingGameScene, loadedAction));

        public static void AsyncAddGameResult(Action<Scene> loadedAction = null) =>Instance.StartCoroutine(Instance.LoadSceneAsyncEnumerator(GameResultScene, loadedAction));
        
        private IEnumerator LoadSceneEnumerator(string sceneName, Action<Scene> loadedAction)
        {
            AsyncOperation asyncOperator = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!asyncOperator.isDone)
            {
                yield return null;
            }
            var scene = SceneManager.GetSceneByName(sceneName);
            loadedAction?.Invoke(scene);
        }
        
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

