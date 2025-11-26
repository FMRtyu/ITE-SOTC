using UnityEngine;

public class GPSTracking : _MenuState
{
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.GPSTracking;
    }
}
