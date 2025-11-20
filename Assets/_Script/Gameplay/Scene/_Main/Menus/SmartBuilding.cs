using UnityEngine;

public class SmartBuilding : _MenuState
{
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.SmartBuilding;
    }
}
