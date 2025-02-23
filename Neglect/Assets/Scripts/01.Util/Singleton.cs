using System;
using UnityEngine;
using Unity.VisualScripting;

namespace Util
{
    public abstract class Singleton<T> : MonoBehaviour, ISingleton where T : Component, new()
    {
        public static T Instance
        {
            get
            {
                Init();
                return _instance;
            }
        }
        private static T _instance = null;
        public static bool HasInstance => _instance;

        private static void Init()
        {
            if (_instance == null)
            {
                var componet = FindObjectOfType<T>();
                if (componet != null)
                {
                    _instance = componet;
                    if(_instance is Singleton<T> s1)
                        s1.Initialize();
                    DontDestroyOnLoad(_instance.gameObject);
                    return;
                }

                var singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                if(_instance is Singleton<T> s)
                    s.Initialize();
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        protected virtual void Initialize() {}
    }
}