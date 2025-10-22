using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventInfo : MonoBehaviour
{
    public TMP_Text eventName;
    public TMP_Text eventVenue;
    public TMP_Text eventTime;
    public TMP_Text eventCrowd;
    public TMP_Text eventStatus;

    public Image eventStatusBG;
    [SerializeField]
    private GameObject[] statusBGSprites;

    public void SetEventInfo(string name, string venue, string time, string crowd, string status)
    {
        eventName.text = name;
        eventVenue.text = venue;
        eventTime.text = time;
        eventCrowd.text = crowd;
        eventStatus.text = status;

        switch (status)
        {
            case "LOW":
                statusBGSprites[0].SetActive(true);
                eventStatus.color = Color.black;
                break;
            case "MEDIUM":
                statusBGSprites[1].SetActive(true);
                eventStatus.color = Color.white;
                break;
            case "HIGH":
                statusBGSprites[2].SetActive(true);
                eventStatus.color = Color.white;
                break;
        }
    }

    public void ClearEventInfo()
    {
        eventName.text = "";
        eventVenue.text = "";
        eventTime.text = "";
        eventCrowd.text = "";
        eventStatus.text = "";
        eventStatusBG.color = Color.clear;
    }

}
