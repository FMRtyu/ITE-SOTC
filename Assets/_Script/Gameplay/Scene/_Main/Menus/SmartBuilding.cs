using UnityEngine;

public class SmartBuilding : _MenuState
{
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.SmartBuilding;
    }
}
