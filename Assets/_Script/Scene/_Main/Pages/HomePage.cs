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

    void SpawnCampusEvent()
    {
        campusEventPanel.RefreshEvents();
    }

}
