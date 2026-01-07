using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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

    public bool isVideoHaveError = false;
    public List<string> videoErrors = new List<string>();

    private readonly string[] modules =
    {
    "module1", "module2", "module3", "module4", "module5"
};

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //LoadOrCreateJson();
        //StartCoroutine(CreateDefaultVideos());
    }

    private async void Start()
    {
        //await LoadCampusEventDataAsync(campusEventJsonURL);

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

    public async void InitializeCampusEventData()
    {
        campusEventJsonURL = Path.Combine(Application.persistentDataPath, "EventSummary.json");
        await LoadCampusEventDataAsync(campusEventJsonURL);
    }
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
        string oldCampusEventJsonURL2 = Path.Combine(Application.persistentDataPath, "Event_Summary.json");

        if (File.Exists(oldCampusEventJsonURL))
        {
            File.Delete(oldCampusEventJsonURL);
            File.Delete(oldCampusEventJsonURL2);
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
            ""event"": ""CCA FAIR 2026"",
            ""venue"": ""FOYER"",
            ""time"": ""1200-1400"",
            ""crowd_size"": ""1200 PAX"",
            ""current_status"": ""LOW""
          },
          {
            ""event"": ""ORIENTATION 2026"",
            ""venue"": ""TES HALL"",
            ""time"": ""0900-1700"",
            ""crowd_size"": ""500 PAX"",
            ""current_status"": ""MEDIUM""
          }
        ]";

        File.WriteAllText(campusEventJsonURL, defaultJson);
        Debug.Log("Default JSON created at: " + campusEventJsonURL);
    }

    // public IEnumerator CreateDefaultVideos()
    // {
    //     string sourceRoot = Path.Combine(Application.streamingAssetsPath, "videos");
    //     string targetRoot = Path.Combine(Application.persistentDataPath, "videos");

    //     Directory.CreateDirectory(targetRoot);

    //     string[] modules = { "module1", "module2", "module3", "module4", "module5" };

    //     foreach (string module in modules)
    //     {
    //         string sourceModule = Path.Combine(sourceRoot, module);
    //         string targetModule = Path.Combine(targetRoot, module);

    //         if (!Directory.Exists(targetModule))
    //         {
    //             Directory.CreateDirectory(targetModule);
    //         }

    //         if (!Directory.Exists(sourceModule))
    //             continue;

    //         foreach (string file in Directory.GetFiles(sourceModule, "*.mp4"))
    //         {
    //             string fileName = Path.GetFileName(file);
    //             string targetFile = Path.Combine(targetModule, fileName);

    //             if (File.Exists(targetFile))
    //                 continue;

    //             yield return CopyFile(file, targetFile);
    //         }

    //         // Copy README.txt (once)
    //         string readmeSource = Path.Combine(sourceRoot, "README.txt");
    //         string readmeTarget = Path.Combine(targetRoot, "README.txt");

    //         if (File.Exists(readmeSource) && !File.Exists(readmeTarget))
    //         {
    //             yield return CopyFile(readmeSource, readmeTarget);
    //         }
    //     }
    // }


    public void CheckDefaultVideos()
    {
        Debug.Log("Checking default videos...");
        videoErrors.Clear();
        isVideoHaveError = false;

        string targetRoot = Path.Combine(Application.persistentDataPath, "videos");

        if (!Directory.Exists(targetRoot))
        {
            Directory.CreateDirectory(targetRoot);
        }

        foreach (string module in modules)
        {
            string modulePath = Path.Combine(targetRoot, module);

            if (!Directory.Exists(modulePath))
            {
                videoErrors.Add($"Missing folder: {module}");
                continue;
            }
            Debug.Log("Checking module: " + module);

            string[] requiredVideos =
{
    "VideoA.mp4",
    "VideoB.mp4",
    "VideoC.mp4"
};

            bool hasAllRequiredVideos = requiredVideos.All(video =>
    File.Exists(Path.Combine(modulePath, video))
);

            if (!hasAllRequiredVideos)
            {
                videoErrors.Add(
                    $"Missing required videos in: {module} (VideoA/B/C.mp4 required)"
                );
            }
        }

        isVideoHaveError = videoErrors.Count > 0;
    }

    public IEnumerator CreateDefaultIncidentVideos()
    {
        videoErrors.Clear();
        isVideoHaveError = false;

        string sourceRoot = Path.Combine(Application.streamingAssetsPath, "videos");
        string targetRoot = Path.Combine(Application.persistentDataPath, "videos");

        Directory.CreateDirectory(targetRoot);

        foreach (string module in modules)
        {
            string sourceModule = Path.Combine(sourceRoot, module);
            string targetModule = Path.Combine(targetRoot, module);

            if (!Directory.Exists(sourceModule))
            {
                videoErrors.Add($"Source module missing: {module}");
                continue;
            }

            Directory.CreateDirectory(targetModule);

            foreach (string file in Directory.GetFiles(sourceModule, "*.mp4"))
            {
                string targetFile = Path.Combine(targetModule, Path.GetFileName(file));

                // if (File.Exists(targetFile))
                //     continue;

                yield return CopyFile(file, targetFile);
            }
        }

        isVideoHaveError = videoErrors.Count > 0;
    }

    public void OpenVideoFolder()
    {
        videoErrors.Clear();
        isVideoHaveError = false;

        string path = Path.Combine(Application.persistentDataPath, "videos");

        if (!Directory.Exists(path))
        {
            videoErrors.Add("Video folder does not exist.");
            isVideoHaveError = true;
            return;
        }

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        };

        System.Diagnostics.Process.Start(psi);
    }

    IEnumerator CopyFile(string sourcePath, string targetPath)
    {
        using UnityWebRequest uwr = UnityWebRequest.Get(sourcePath);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            string msg = $"Copy failed: {Path.GetFileName(sourcePath)} | {uwr.error}";
            Debug.LogError(msg);

            videoErrors.Add(msg);
            isVideoHaveError = true;
        }
        else
        {
            File.WriteAllBytes(targetPath, uwr.downloadHandler.data);
            Debug.Log("Copied: " + targetPath);
        }
    }
}
