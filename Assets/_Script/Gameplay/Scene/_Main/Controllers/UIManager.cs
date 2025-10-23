using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Game Controllers")]
    [SerializeField] private GameManager gameManager;
    
    [Header("Pages")]
    [SerializeField] private LandingController landingPage;
    private CanvasGroup landingCanvasGroup;
    [SerializeField] private DashboardController dashboardPage;
    private CanvasGroup dashboardCanvasGroup;

    [Header("HUD Elements")]
    [SerializeField] private GameObject HUD;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initUIElement();
    }

    private void initUIElement()
    {
        landingCanvasGroup = landingPage.GetComponent<CanvasGroup>();
        dashboardCanvasGroup = dashboardPage.GetComponent<CanvasGroup>();

        dashboardCanvasGroup.alpha = 0f;
        dashboardCanvasGroup.interactable = false;
        dashboardCanvasGroup.blocksRaycasts = false;

        PopOutFade();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDashboard()
    {
        dashboardCanvasGroup.alpha = 1f;
        dashboardCanvasGroup.interactable = true;
        dashboardCanvasGroup.blocksRaycasts = true;

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
