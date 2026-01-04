using UnityEngine;
using SerapKeremGameKit._Logging;

namespace SerapKeremGameKit._Singletons
{
    /// <summary>
    /// A base class for creating MonoSingletons, which are limited to a single instance in a scene.
    /// </summary>
    /// <typeparam name="T">The type of the MonoSingleton.</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        /// <summary>
        /// The single instance of this MonoSingleton.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    TraceLogger.LogError($"Instance of {typeof(T).Name} is not found in the scene.");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Indicates whether the MonoSingleton instance is initialized.
        /// </summary>
        public static bool IsInitialized => _instance != null;

        /// <summary>
        /// Ensures only one instance of the MonoSingleton exists in the scene.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                TraceLogger.LogWarning($"Duplicate instance of {typeof(T).Name} detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
        }

        /// <summary>
        /// Clears the instance on destruction.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
