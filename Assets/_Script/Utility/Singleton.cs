using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // Kalau belum ada instance, cari di scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                // Kalau masih tidak ada, buat baru
                if (_instance == null)
                {
                    GameObject singletonObj = new GameObject(typeof(T).Name);
                    _instance = singletonObj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // Cegah duplikasi
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // Optional: biar tidak hilang saat ganti scene
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
