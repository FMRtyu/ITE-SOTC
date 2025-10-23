using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initUIElement();
    }

    private void initUIElement()
    {
        HUD.alpha = 0f;
        HUD.interactable = false;
        HUD.blocksRaycasts = false;

        landingCanvasGroup = landingPage.GetComponent<CanvasGroup>();
        dashboardCanvasGroup = dashboardPage.GetComponent<CanvasGroup>();

        dashboardCanvasGroup.interactable = false;
        dashboardCanvasGroup.blocksRaycasts = false;

        PopOutFade();
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
