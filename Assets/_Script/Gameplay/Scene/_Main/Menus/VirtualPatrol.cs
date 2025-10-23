using UnityEngine;

public class VirtualPatrol : _MenuState
{
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.VirtualPatrol;
    }
}
