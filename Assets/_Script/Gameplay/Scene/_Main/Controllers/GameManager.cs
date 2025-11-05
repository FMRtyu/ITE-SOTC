using System.IO;
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

    //load or create new json
    public CampusEventData[] eventsData;
    private string campusEventJsonURL;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        campusEventJsonURL = Path.Combine(Application.persistentDataPath, "crowd_data.json");
        LoadOrCreateJson();
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

    private void LoadOrCreateJson()
    {
        if (!File.Exists(campusEventJsonURL))
        {
            Debug.Log("JSON not found, creating new one...");
            CreateDefaultJson();
        }

        string json = File.ReadAllText(campusEventJsonURL);
        eventsData = JsonHelper.FromJson<CampusEventData>(json);

        Debug.Log("Loaded " + eventsData.Length + " campus events");
    }

    private void CreateDefaultJson()
    {
        string defaultJson = @"
        [
          {
            ""event"": ""CCA Fair 2025"",
            ""venue"": ""FOYER"",
            ""time"": ""12PM - 2PM"",
            ""crowd_size"": ""100 PAX"",
            ""current_status"": ""LOW""
          },
          {
            ""event"": ""Open House 2025"",
            ""venue"": ""Auditorium"",
            ""time"": ""10AM - 1PM"",
            ""crowd_size"": ""200 PAX"",
            ""current_status"": ""MEDIUM""
          }
        ]";

        File.WriteAllText(campusEventJsonURL, defaultJson);
        Debug.Log("Default JSON created at: " + campusEventJsonURL);
    }
}
