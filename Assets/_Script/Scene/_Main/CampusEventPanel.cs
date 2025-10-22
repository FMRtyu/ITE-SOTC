using System.Collections.Generic;
using UnityEngine;

public class CampusEventPanel : MonoBehaviour
{
    [SerializeField] private MainController mainController;


    [SerializeField]
    private GameObject eventPanelPrefab;
    private List<GameObject> eventPanels = new List<GameObject>();
    [SerializeField]
    private Transform eventPanelParent;

    public void RefreshEvents()
    {
        foreach (GameObject eventPanel in eventPanels)
        {
            Destroy(eventPanel);
        }
        eventPanels.Clear();
        SpawnCampusEventList();
    }

    void SpawnCampusEventList()
    {
        foreach (EventData e in mainController.campusEvent)
        {
            GameObject temp = Instantiate(eventPanelPrefab, eventPanelParent);

            temp.GetComponent<EventInfo>().SetEventInfo(e.@event, e.venue, e.time, e.crowd_size, e.current_status);

            eventPanels.Add(temp);
        }

        if (eventPanels.Count < 2)
        {
            for (int i = eventPanels.Count; i < 2; i++)
            {
                GameObject temp = Instantiate(eventPanelPrefab, eventPanelParent);
                temp.GetComponent<EventInfo>().ClearEventInfo();

                eventPanels.Add(temp);
            }
        }
    }
}
