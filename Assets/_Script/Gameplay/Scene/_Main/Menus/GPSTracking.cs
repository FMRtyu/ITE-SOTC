using UnityEngine;

public class GPSTracking : _MenuState
{
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.GPSTracking;
    }
}
