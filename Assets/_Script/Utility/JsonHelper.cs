using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.items;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        string json = JsonUtility.ToJson(wrapper, prettyPrint);
        return json.Replace("{\"items\":", "").TrimEnd('}');
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
