using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    public HostGameManager GameManager { get; private set; }

    private static HostSingleton instance;
    public static HostSingleton Instance {
        get
        {
            if (instance != null) { return instance; }
            instance = FindFirstObjectByType<HostSingleton>();

            if (instance == null)
            {
                // Debug.LogError("No HostSingleton in the scene!");
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
}