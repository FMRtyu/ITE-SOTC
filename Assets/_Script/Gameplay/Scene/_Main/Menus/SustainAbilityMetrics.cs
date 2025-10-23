using UnityEngine;

public class SustainAbilityMetrics : _MenuState
{
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.SustainabilityMetrics;
    }
}
