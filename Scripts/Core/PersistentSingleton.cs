using UnityEngine;

namespace Core
{
    /// <summary>
    /// Creates an instance of the class, and prevents it from being destroyed when loading a new scene.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
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

                    DontDestroyOnLoad(instance);

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
            // If the singleton instance isn't null and isn't this instance, destroy this instance
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"Destroying duplicate instance of {typeof(T)}");
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

    }
}