using System.Collections.Generic;
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
    [SerializeField] private GameObject smartbuildButton;
    [SerializeField] private GameObject sustainabilityButton;
    [SerializeField] private GameObject HomeButton;
    [SerializeField] private GameObject GPSTrackingButton;
    [SerializeField] private GameObject alarmMonitoringButton;
    [SerializeField] private GameObject virtualPatrolButton;

    private List<GameObject> navGroup = new List<GameObject>();

    [Header("debug")]
    [SerializeField] private bool skipLanding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initUIElement();

        navGroup.Add(smartbuildButton);
        navGroup.Add(sustainabilityButton);
        navGroup.Add(HomeButton);
        navGroup.Add(GPSTrackingButton);
        navGroup.Add(alarmMonitoringButton);
        navGroup.Add(virtualPatrolButton);

        activeBTN(HomeButton);
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

    public void OpenMenu(int index)
    {
        MenuState selectedMenu = MenuState.Home;
        GameObject selectedNavButton = null;
        switch (index)
        {
            case 0:
                selectedMenu = MenuState.SmartBuilding;
                selectedNavButton = smartbuildButton;
                break;
            case 1:
                selectedMenu = MenuState.SustainabilityMetrics;
                selectedNavButton = sustainabilityButton;
                break;
            case 2:
                selectedMenu = MenuState.Home;
                selectedNavButton = HomeButton;
                break;
            case 3:
                selectedMenu = MenuState.GPSTracking;
                selectedNavButton = GPSTrackingButton;
                break;
            case 4:
                selectedMenu = MenuState.AlarmMonitoring;
                selectedNavButton = alarmMonitoringButton;
                break;
            case 5:
                selectedMenu = MenuState.VirtualPatrol;
                selectedNavButton = virtualPatrolButton;
                break;
        }

        if (selectedMenu != dashboardPage.currentMenuState)
        {
            dashboardPage.SetActiveState(selectedMenu);
            activeBTN(selectedNavButton);
            SoundManager.Instance.PlaySFX("button_click");
        }

    }

    public void activeBTN(GameObject btn)
    {
        foreach (GameObject item in navGroup)
        {
            deactiveBTN(item);
        } 
        btn.transform.GetChild(0).gameObject.SetActive(true);
        btn.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void deactiveBTN(GameObject btn)
    {
        btn.transform.GetChild(0).gameObject.SetActive(false);
        btn.transform.GetChild(1).gameObject.SetActive(true);
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

            SoundManager.Instance.SetVolume(0.2f, "theEpic", 1f);
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
