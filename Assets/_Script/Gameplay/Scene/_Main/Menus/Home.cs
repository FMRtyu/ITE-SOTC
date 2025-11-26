using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : _MenuState
{
    [Header("UI Elements")]
    [Header("Campus Event Panel")]
    [SerializeField] private GameObject campusEventPanel;
    [SerializeField] private GameObject campusContentPrefab;
    [SerializeField] private GameObject campusContentParent;
    private List<GameObject> campusEventContents = new List<GameObject>();
    //Specific for this state
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.Home;
    }

    void Start()
    {
        HideAllChildren();

        initHome();
    }

    private void HideAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void initHome()
    {
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("button_click");
            });
        }
    }
    
    public void ShowChildren()
    {
        // Enable all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ShowCampusEventOnly()
    {
        // Enable campus event panel
        campusEventPanel.SetActive(true);
        LeanTween.alphaCanvas(campusEventPanel.GetComponent<CanvasGroup>(), 1f, 0.5f).setDelay(3f).setOnComplete(() =>
        {
            SetCampusEventData();
        });

    }
    
    private void SetCampusEventData()
    {
        foreach (GameObject eventPanel in campusEventContents)
        {
            Destroy(eventPanel);
        }
        campusEventContents.Clear();

        foreach (CampusEventData Data in GameManager.Instance.CampusEvents)
        {
            GameObject temp = Instantiate(campusContentPrefab, campusContentParent.transform);

            temp.GetComponent<EventInfo>().SetEventInfo(Data.@event, Data.venue, Data.time, Data.crowd_size, Data.current_status);

            campusEventContents.Add(temp);
        }
    }
}
