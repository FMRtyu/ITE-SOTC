using UnityEngine;

[System.Serializable]
public class CampusEventData
{
    public string @event; // the '@' allows using the keyword "event"
    public string venue;
    public string time;
    public string crowd_size;
    public string current_status;
}

[System.Serializable]
public class AudioClipData
{
    public string clipName;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float clipVolume;

    public bool loop;
}
