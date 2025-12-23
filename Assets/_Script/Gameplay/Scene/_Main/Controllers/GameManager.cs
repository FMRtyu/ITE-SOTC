using System.IO;
using System.Text.RegularExpressions;
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
    //public CampusEventData[] eventsData;
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

        campusEventJsonURL = Path.Combine(Application.persistentDataPath, "Event_Summary.json");
        //LoadOrCreateJson();
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
        if (!File.Exists(url))
         {
             Debug.Log("JSON not found, creating new one...");
            CreateDefaultJson();
         }
        CampusEvents = await JsonFetcherUtility.FetchArrayAsync<CampusEventData>(url);

        ChangeJSONFont();

        if (CampusEvents == null)
        {
            Debug.LogWarning("[DataManager] Campus event data failed to load or is empty.");
        }
        else
        {
            Debug.Log($"[DataManager] Loaded {CampusEvents.Length} campus events.");
        }

        //delete old json
        string oldCampusEventJsonURL = Path.Combine(Application.persistentDataPath, "crowd_data.json");

        if (File.Exists(oldCampusEventJsonURL))
        {
            File.Delete(oldCampusEventJsonURL);
        }
    }

    // private void LoadOrCreateJson()
    // {
    //     if (!File.Exists(campusEventJsonURL))
    //     {
    //         Debug.Log("JSON not found, creating new one...");
    //         CreateDefaultJson();
    //     }

    //     string json = File.ReadAllText(campusEventJsonURL);
    //     eventsData = JsonHelper.FromJson<CampusEventData>(json);

    //     Debug.Log("Loaded " + eventsData.Length + " campus events");

    //     ChangeJSONFont();

    //     //delete old json
    //     string oldCampusEventJsonURL = Path.Combine(Application.persistentDataPath, "crowd_data.json");

    //     if (File.Exists(oldCampusEventJsonURL))
    //     {
    //         File.Delete(oldCampusEventJsonURL);
    //     }
    // }

    void ChangeJSONFont()
    {
        foreach (CampusEventData Data in CampusEvents)
        {
            Data.@event = ConvertYearFontEvent(Data.@event);
            Data.@crowd_size = ConvertYearFontcrowd(Data.@crowd_size);
        }
    }

    string ConvertYearFontcrowd(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;

        string numberFont = "Oswald-Medium SDF";

        // Cari angka di awal
        var match = System.Text.RegularExpressions.Regex.Match(raw, @"^(\d+)(.*)$");

        if (match.Success)
        {
            string number = match.Groups[1].Value;
            string rest = match.Groups[2].Value;

            return $"<font=\"{numberFont}\">{number}</font>{rest}";
        }

        return raw; // kalau tidak ada angka di awal
    }

    string ConvertYearFontEvent(string input)
    {
        return Regex.Replace(
            input,
            @"(\d+)$", // angka di bagian paling akhir string
            m => $"<font=\"Oswald-Medium SDF\">{m.Value}</font>"
        );
    }

    private void CreateDefaultJson()
    {
        string defaultJson = @"
        [
          {
            ""event"": ""CCA FAIR 2025"",
            ""venue"": ""FOYER"",
            ""time"": ""1200-1400"",
            ""crowd_size"": ""1200 PAX"",
            ""current_status"": ""LOW""
          },
          {
            ""event"": ""ORIENTATION 2025"",
            ""venue"": ""TES HALL"",
            ""time"": ""0900-1700"",
            ""crowd_size"": ""500 PAX"",
            ""current_status"": ""MEDIUM""
          }
        ]";

        File.WriteAllText(campusEventJsonURL, defaultJson);
        Debug.Log("Default JSON created at: " + campusEventJsonURL);
    }
}
