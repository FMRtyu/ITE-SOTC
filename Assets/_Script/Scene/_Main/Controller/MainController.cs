using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private CanvasGroup landingScene;
    [SerializeField] private MainDashboard dashboardScene;

    [SerializeField]
    private string jsonUrl = "file:///D:/Assets/CampusDataEvent.json";  // local path
    public EventData[] campusEvent;

    void Start()
    {
        dashboardScene.initDashboard();
        initData();
    }

    void initData()
    {
        // Any data initialization if needed
        GetJSONData.SetJsonUrl(jsonUrl);

        campusEvent = GetJSONData.FetchEventDataAsync().Result;

        foreach (EventData e in campusEvent)
        {
            Debug.Log($"Event: {e.@event}, Venue: {e.venue}, Time: {e.time}, Crowd Size: {e.crowd_size}, Status: {e.current_status}");
        }
    }
}
