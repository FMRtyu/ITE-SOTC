using UnityEngine;

public class SustainAbilityMetrics : _MenuState
{
    [SerializeField] private Sprite menuBG;

    void OnEnable()
    {
        if (uiManager != null)
        {
            uiManager.ChangeBackground(menuBG);
            uiManager.ShowBlackBG(3);
        }
    }
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.SustainabilityMetrics;
    }
}
