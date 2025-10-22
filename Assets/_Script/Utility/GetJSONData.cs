using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GetJSONData : MonoBehaviour
{
    private static string jsonUrl = "YOUR_JSON_URL_HERE";

    /// <summary>
    /// Fetches and returns event data from the given JSON URL.
    /// </summary>
    public static async Task<EventData[]> FetchEventDataAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
        {
#if UNITY_2020_1_OR_NEWER
            var operation = request.SendWebRequest();
#else
            var operation = request.Send();
#endif
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                EventData[] events = JsonHelper.FromJson<EventData>(json);
                return events;
            }
            else
            {
                Debug.LogError("Failed to fetch JSON: " + request.error);
                return null;
            }
        }
    }
    public static void SetJsonUrl(string url)
    {
        jsonUrl = url;
    }
}

/// <summary>
/// Helper for JSON arrays (since Unity’s JsonUtility doesn’t handle arrays directly)
/// </summary>
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
