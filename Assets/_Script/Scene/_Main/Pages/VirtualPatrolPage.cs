using UnityEngine;

public class VirtualPatrolPage : _MenuState
{
    public override void InitState(MainDashboard dashboardController)
    {
        base.InitState(dashboardController);

        state = PageNavType.virtualPatrol;
    }
}
