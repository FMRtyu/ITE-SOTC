using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState;
    [Header("Managers")]
    public UIManager uIManager;

    [Header("Campus Event Data")]
    public CampusEventData[] CampusEvents;
    [SerializeField] private string campusEventJsonURL;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await LoadCampusEventDataAsync(campusEventJsonURL);

        var events = CampusEvents;

        if (events != null)
        {
            foreach (var e in events)
                Debug.Log($"Event: {e.@event} | Venue: {e.venue} | Time: {e.time}");
        }
    }

    /// <summary>
    /// Fetches and stores campus event data from the given JSON URL.
    /// </summary>
    /// <summary>
    /// Fetches and stores campus event data from the given JSON URL.
    /// </summary>
    public async Task LoadCampusEventDataAsync(string url)
    {
        CampusEvents = await JsonFetcherUtility.FetchArrayAsync<CampusEventData>(url);

        if (CampusEvents == null)
        {
            Debug.LogWarning("[DataManager] Campus event data failed to load or is empty.");
        }
        else
        {
            Debug.Log($"[DataManager] Loaded {CampusEvents.Length} campus events.");
        }
    }
}
