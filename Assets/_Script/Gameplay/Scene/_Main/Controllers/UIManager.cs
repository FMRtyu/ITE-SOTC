using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("Pages")]
    [SerializeField] private LandingController landingPage;
    private CanvasGroup landingCanvasGroup;
    [SerializeField] private DashboardController dashboardPage;
    private CanvasGroup dashboardCanvasGroup;

    [Header("HUD Elements")]
    [SerializeField] private CanvasGroup HUD;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [Header("Navigation elements")]
    [SerializeField] private Button smartbuildButton;
    [SerializeField] private Button sustainabilityButton;
    [SerializeField] private Button HomeButton;
    [SerializeField] private Button GPSTrackingButton;
    [SerializeField] private Button alarmMonitoringButton;
    [SerializeField] private Button virtualPatrolButton;

    [Header("debug")]
    [SerializeField] private bool skipLanding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initUIElement();
        InitButton();
    }

    private void initUIElement()
    {
        landingCanvasGroup = landingPage.GetComponent<CanvasGroup>();
        dashboardCanvasGroup = dashboardPage.GetComponent<CanvasGroup>();

        if (skipLanding)
        {
            ShowDashboardCampusEvent();
            ShowDashboard();
        }
        else
        {
            HUD.alpha = 0f;
            HUD.interactable = false;
            HUD.blocksRaycasts = false;

            dashboardCanvasGroup.interactable = false;
            dashboardCanvasGroup.blocksRaycasts = false;
        }
        PopOutFade();


    }

    void InitButton()
    {
        HomeButton.interactable = false;
        HomeButton.onClick.AddListener(() =>
        {
            dashboardPage.SetActiveState(MenuState.Home);
            HomeButton.interactable = false;
            virtualPatrolButton.interactable = true;

            SoundManager.Instance.PlaySFX("button_click");
        });
        virtualPatrolButton.onClick.AddListener(() =>
        {
            dashboardPage.SetActiveState(MenuState.VirtualPatrol);
            virtualPatrolButton.interactable = false;
            HomeButton.interactable = true;

            SoundManager.Instance.PlaySFX("button_click");
        });
    }

    public void ShowDashboardCampusEvent()
    {
        dashboardPage.showCampusEventPanelOnly();
    }

    public void ShowDashboard()
    {
        dashboardCanvasGroup.interactable = true;
        dashboardCanvasGroup.blocksRaycasts = true;
        dashboardPage.ShowAllHomeChildren();
        HUD.alpha = 1f;
        HUD.interactable = true;
        HUD.blocksRaycasts = true;

        LeanTween.alphaCanvas(landingCanvasGroup, 0f, 0.5f).setOnComplete(() =>
        {
            landingCanvasGroup.interactable = false;
            landingCanvasGroup.blocksRaycasts = false;
        });

    }
    #region Fade Controls
    public void PopInFade()
    {
        LeanTween.alphaCanvas(fadeCanvasGroup, 1f, 2f).setOnComplete(() =>
        {
            fadeCanvasGroup.interactable = true;
            fadeCanvasGroup.blocksRaycasts = true;
        });
    }

    public void PopOutFade()
    {
        LeanTween.alphaCanvas(fadeCanvasGroup, 0f, 2f).setOnComplete(() =>
        {
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        });
    }
    #endregion
}
