public enum PageNavType
{
    smartBuilding,
    sustainableMetrics,
    home,
    gpsTracking,
    alarmMonitoring,
    virtualPatrol,
}

public enum CCTVFeed
{
    field,
    pool,
    blockB,
    blockE,

}

[System.Serializable]
public class EventData
{
    public string @event; // the '@' allows using the keyword "event"
    public string venue;
    public string time;
    public string crowd_size;
    public string current_status;
}