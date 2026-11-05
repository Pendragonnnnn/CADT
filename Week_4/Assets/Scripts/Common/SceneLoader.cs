using UnityEngine.SceneManagement;

namespace PhysicsDemo
{
    /// <summary>Loads scenes and always restores normal time speed first.</summary>
    public static class SceneLoader
    {
        /// <summary>True when the scene now loading was started by <see cref="Reload"/>, so scripts can keep their state (e.g. the camera view).</summary>
        public static bool IsReload { get; private set; }

        // Statics survive Play mode when domain reload is off: start every Play session clean.
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => IsReload = false;

        /// <summary>Loads a scene listed in Build Settings by name.</summary>
        public static void Load(string sceneName) => Load(sceneName, false);

        static void Load(string sceneName, bool reload)
        {
            if (!IsAvailable(sceneName))
            {
                UnityEngine.Debug.LogWarning($"Scene '{sceneName}' is not in Build Settings. Add it in File > Build Profiles.");
                return;
            }
            TimeControl.ResetToNormal();
            IsReload = reload;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>True if the scene is listed in Build Settings and can be loaded by name.</summary>
        public static bool IsAvailable(string sceneName) => UnityEngine.Application.CanStreamedLevelBeLoaded(sceneName);

        /// <summary>Reloads the active scene: the simplest and most reliable "reset". The camera keeps its view.</summary>
        public static void Reload() => Load(SceneManager.GetActiveScene().name, true);
    }
}
