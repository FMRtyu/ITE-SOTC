using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class JsonFetcherUtility
{
    /// <summary>
    /// Fetches JSON from a URL and deserializes it into an array of type T.
    /// </summary>
    public static async Task<T[]> FetchArrayAsync<T>(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return JsonHelper.FromJson<T>(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[JsonFetcherUtility] Failed to fetch JSON from {url}: {request.error}");
                return null;
            }
        }
    }

    /// <summary>
    /// Fetches JSON from a URL and deserializes it into a single object of type T.
    /// </summary>
    public static async Task<T> FetchObjectAsync<T>(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return JsonUtility.FromJson<T>(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[JsonFetcherUtility] Failed to fetch JSON from {url}: {request.error}");
                return default;
            }
        }
    }

    /// <summary>
    /// Reads JSON from a local file path (inside StreamingAssets or persistent data).
    /// </summary>
    public static async Task<string> LoadLocalJsonAsync(string filePath)
    {
        string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, filePath);

        using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
                return request.downloadHandler.text;
            else
            {
                Debug.LogError($"[JsonFetcherUtility] Failed to load local JSON at {fullPath}: {request.error}");
                return null;
            }
        }
    }
    public static class JsonHelper
    {
        /// <summary>
        /// Allows parsing a JSON array directly using JsonUtility.
        /// </summary>
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }
}
