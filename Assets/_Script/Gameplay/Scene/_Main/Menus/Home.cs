using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : _MenuState
{
    [Header("UI Elements")]
    [SerializeField] private Sprite menuBG;
    [SerializeField] private Animator PieChartAnimator;
    [Header("Campus Event Panel")]
    [SerializeField] private GameObject campusContentPrefab;
    [SerializeField] private GameObject campusContentParent;
    private List<GameObject> campusEventContents = new List<GameObject>();

    bool isInit = false;
    //Specific for this state
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.Home;
    }

    void Start()
    {
        if (!uiManager.skipLanding)
            HideAllChildren();

        initHome();
    }

    void OnEnable()
    {
        if (uiManager != null)
        {
            uiManager.ChangeBackground(menuBG);
            uiManager.ShowBlackBG(0);
        }

        PieChartAnimator.SetBool("Init", isInit);
    }

    void OnDisable()
    {
        if (!isInit)
        {
            isInit = true;
        }
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

        uiManager.ChangeBackground(menuBG);
    }

    public void SetCampusEventData()
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
