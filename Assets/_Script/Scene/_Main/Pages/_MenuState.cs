using UnityEngine;

public class _MenuState : MonoBehaviour
{
    //Which state is this?
    public PageNavType state { get; protected set; }


    protected MainDashboard dashboardController;


    //Dependency injection of the MenuController to make it easier to reference it from each menu
    public virtual void InitState(MainDashboard DashboardController)
    {
        this.dashboardController = DashboardController;
    }


    //Jump back to the menu before it when we press a back button or escape key
    //You have to manually hook up each back-button to this method
    public void JumpBack()
    {
        dashboardController.JumpBack();
    }
}
