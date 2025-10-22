using UnityEngine;

public class HomePage : _MenuState
{
    [SerializeField] private GameObject[] preHomeUIElements;
    [SerializeField] private CampusEventPanel campusEventPanel;
    public override void InitState(MainDashboard dashboardController)
    {
        base.InitState(dashboardController);

        state = PageNavType.home;
    }

    void Start()
    {
        foreach (GameObject child in transform)
        {
            child.SetActive(false);
        }
    }

    public void activatedPreHome()
    {
        foreach (GameObject element in preHomeUIElements)
        {
            element.SetActive(true);
        }
        campusEventPanel.RefreshEvents();
    }

    public void ActivateMainHome()
    {
        foreach (GameObject child in transform)
        {
            child.SetActive(true);
        }
    }
}
