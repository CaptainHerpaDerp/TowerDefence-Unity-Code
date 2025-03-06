using UnityEngine;

namespace Core
{
    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                // If the instance is null, try to find an object of type T in the scene
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();

                    // If the instance is still null, log an error
                    if (instance == null)
                    {
                        Debug.LogWarning($"An instance of {typeof(T)} is needed in the scene, but there is none.");
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Debug.LogWarning($"Multiple instances of {typeof(T)} detected. Destroying the newest one.");
                Destroy(gameObject);
            }
        }
    }
}