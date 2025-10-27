using UnityEngine;

public class AlarmMonitoring : _MenuState
{
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.AlarmMonitoring;
    }
}
