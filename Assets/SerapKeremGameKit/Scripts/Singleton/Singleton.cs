namespace SerapKeremGameKit._Singletons
{
    /// <summary>
    /// A generic Singleton class that works independently of MonoBehaviour and across the entire game lifecycle.
    /// </summary>
    /// <typeparam name="T">The type of the Singleton.</typeparam>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly object _lock = new object();
        private static T _instance;

        /// <summary>
        /// The single instance of this Singleton.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            _instance.Initialize();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Protected constructor to prevent external instantiation.
        /// </summary>
        protected Singleton() { }

        /// <summary>
        /// Initializes the Singleton instance. Called during instance creation.
        /// </summary>
        protected virtual void Initialize() { }
    }
}