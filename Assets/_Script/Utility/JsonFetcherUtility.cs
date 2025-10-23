using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class JsonFetcherUtility
{
    /// <summary>
    /// Fetches JSON data from a given URL and deserializes it into an array of type T.
    /// Works universally for any class that can be serialized with JsonUtility.
    /// </summary>
    public static async Task<T[]> FetchJsonArrayAsync<T>(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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
                T[] dataArray = JsonHelper.FromJson<T>(json);
                return dataArray;
            }
            else
            {
                Debug.LogError($"Failed to fetch JSON from {url}: {request.error}");
                return null;
            }
        }
    }

    /// <summary>
    /// Fetches JSON data from a given URL and deserializes it into a single object of type T.
    /// Useful if the JSON represents a single entity instead of an array.
    /// </summary>
    public static async Task<T> FetchJsonObjectAsync<T>(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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
                T data = JsonUtility.FromJson<T>(json);
                return data;
            }
            else
            {
                Debug.LogError($"Failed to fetch JSON from {url}: {request.error}");
                return default;
            }
        }
    }
}

/// <summary>
/// Helper for converting JSON arrays (since Unity’s JsonUtility doesn’t handle arrays directly).
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
