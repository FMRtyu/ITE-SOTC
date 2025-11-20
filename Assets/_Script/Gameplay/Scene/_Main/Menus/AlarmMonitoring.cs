using UnityEngine;

public class AlarmMonitoring : _MenuState
{
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.AlarmMonitoring;
    }
}
