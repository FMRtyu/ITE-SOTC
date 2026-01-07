using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{

    [Header("Pages")]
    [SerializeField] private LandingController landingPage;
    private CanvasGroup landingCanvasGroup;
    private DashboardController dashboardPage;
    private CanvasGroup dashboardCanvasGroup;

    [Header("HUD Elements")]
    [SerializeField] private CanvasGroup HUD;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private CanvasGroup backgroundCG;
    [SerializeField] private GameObject muteIcon;
    [SerializeField] private GameObject[] blackBG;
    [SerializeField] private float delayMenuSwitch = 1f;
    private bool isChangingMenu = false;

    [Header("Time & Date Elements")]
    [SerializeField] private TMP_Text hourText;
    [SerializeField] private TMP_Text minuteText;
    [SerializeField] private TMP_Text secondText;
    [SerializeField] private TMP_Text dateText;

    [Header("Navigation elements")]
    [SerializeField] private GameObject smartbuildButton;
    [SerializeField] private GameObject sustainabilityButton;
    [SerializeField] private GameObject HomeButton;
    [SerializeField] private GameObject GPSTrackingButton;
    [SerializeField] private GameObject alarmMonitoringButton;
    [SerializeField] private GameObject virtualPatrolButton;

    [Header("loadingUI")]
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private Slider loadingSlider;

    [Header("Virtual Patrol Elements")]
    public CanvasGroup pinPointCG;
    public CanvasGroup virtualBGCG;
    public CanvasGroup popupBlackBGCG;
    public VideoPlayer popupVideoPlayer;


    private bool isShowing = false;

    private int changingID = -1;

    public bool isplayingBGM = true;

    private List<GameObject> navGroup = new List<GameObject>();

    [Header("debug")]
    public bool skipLanding = false;

    //var
    private DateTime today;

    private Sprite defaultBG;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        if (skipLanding)
        {
            //ShowDashboard();
            initUIManager();
            GameManager.Instance.InitializeCampusEventData();
            loadDashboardScene();
        }
    }
    public void initUIManager()
    {
        today = DateTime.Now;
        landingCanvasGroup = landingPage.GetComponent<CanvasGroup>();
        // Format: 6th October 2025 Friday
        string formatted =
    today.ToString("dddd, d MMMM").ToUpper()
    + $" <font=\"Oswald-Medium SDF\"><size=21>{today:yyyy}</size></font>";

        dateText.text = formatted;


        HUD.alpha = 0f;
        HUD.interactable = false;
        HUD.blocksRaycasts = false;
        StartCoroutine(UpdateTime());

        navGroup.Add(smartbuildButton);
        navGroup.Add(sustainabilityButton);
        navGroup.Add(HomeButton);
        navGroup.Add(GPSTrackingButton);
        navGroup.Add(alarmMonitoringButton);
        navGroup.Add(virtualPatrolButton);

        activeNavBTN(HomeButton);

        defaultBG = backgroundCG.GetComponent<Image>().sprite;

    }

    public void InitDashboardData(DashboardController dashboard)
    {
        dashboardPage = dashboard;
        dashboardCanvasGroup = dashboardPage.GetComponent<CanvasGroup>();
    }

    IEnumerator UpdateTime()
    {
        while (true)
        {
            DateTime now = DateTime.Now;

            hourText.text = now.ToString("HH");
            minuteText.text = now.ToString("mm");
            secondText.text = now.ToString("ss");

            yield return new WaitForSeconds(1f);
        }
    }

    public void OpenMenu(int index)
    {
        if (isChangingMenu || dashboardPage.CheckScenarioInProgress())
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }
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
            Debug.Log("Changing Menu to: " + selectedMenu.ToString());
            dashboardPage.SetActiveState(selectedMenu);
            activeNavBTN(selectedNavButton);
            SoundManager.Instance.PlaySFX("button_click");
            delayChangeMenu();
        }

    }

    void delayChangeMenu()
    {
        isChangingMenu = true;
        LeanTween.delayedCall(delayMenuSwitch, () =>
        {
            isChangingMenu = false;
        });
    }

    public void activeNavBTN(GameObject btn)
    {
        foreach (GameObject item in navGroup)
        {
            deactiveBTN(item);
        }

        if (btn.transform.childCount >= 2)
        {
            btn.transform.GetChild(0).gameObject.SetActive(true);
            btn.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void deactiveBTN(GameObject btn)
    {
        if (btn.transform.childCount >= 2)
        {
            btn.transform.GetChild(0).gameObject.SetActive(false);
            btn.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void ShowDashboard()
    {
        dashboardCanvasGroup.interactable = true;
        dashboardCanvasGroup.blocksRaycasts = true;
        dashboardPage.showCampusEventPanelOnly();
        dashboardPage.ShowAllHomeChildren();
        HUD.alpha = 1f;
        HUD.interactable = true;
        HUD.blocksRaycasts = true;

        LeanTween.alphaCanvas(landingCanvasGroup, 0f, 0.5f).setOnComplete(() =>
        {
            landingCanvasGroup.interactable = false;
            landingCanvasGroup.blocksRaycasts = false;

            //toggleBGM(false);
            SoundManager.Instance.PlaySFX("MenuOpen");
            SoundManager.Instance.SetVolume(0.02f, "theEpic", 1f);
            dashboardPage.CheckVideoIncident();
        });

    }

    public void ChangeBackground(Sprite newBG)
    {
        LeanTween.cancel(changingID);

        Image bgImage = backgroundCG.GetComponent<Image>();

        if (newBG == bgImage.sprite)
            return;
        if (dashboardPage.currentMenuState == MenuState.VirtualPatrol)
        {
            bgImage.sprite = newBG;
        }
        else
        {
            changingID = LeanTween.alphaCanvas(backgroundCG, 0f, 1f).setOnComplete(() =>
        {
            if (newBG == null)

                bgImage.sprite = defaultBG;
            else
                bgImage.sprite = newBG;
            changingID = LeanTween.alphaCanvas(backgroundCG, 1f, 1f).id;
        }).id;
        }
    }

    public void toggleBGM()
    {
        if (!isplayingBGM)
        {
            SoundManager.Instance.SetVolume(0.02f, "theEpic", 1f);
            isplayingBGM = true;
            muteIcon.SetActive(false);
        }
        else if (isplayingBGM)
        {
            SoundManager.Instance.SetVolume(0f, "theEpic", 1f);
            isplayingBGM = false;
            muteIcon.SetActive(true);
        }
    }

    public void toggleBGM(bool play)
    {
        if (play)
        {
            SoundManager.Instance.SetVolume(0.02f, "theEpic", 1f);
            isplayingBGM = true;
            muteIcon.SetActive(false);
        }
        else if (!play)
        {
            SoundManager.Instance.SetVolume(0f, "theEpic", 1f);
            isplayingBGM = false;
            muteIcon.SetActive(true);
        }
    }

    public void ShowBlackBG(int amount)
    {
        for (int i = 0; i < blackBG.Length; i++)
        {
            if (i < amount)
            {
                blackBG[i].SetActive(true);
            }
            else
            {
                blackBG[i].SetActive(false);
            }
        }
    }

    public void loadDashboardScene()
    {
        LeanTween.alphaCanvas(loadingCanvasGroup, 1f, 0.5f).setOnComplete(() =>
        {
            StartCoroutine(LoadGameplayRoutine());
        });
    }

    IEnumerator LoadGameplayRoutine()
    {
        // Reset slider
        loadingSlider.value = 0f;

        // 🔑 KASIH WAKTU UI & LEANTWEEN RENDER
        yield return null;
        yield return null;

        // Load scene async (Additive supaya loading scene tetap hidup)
        AsyncOperation op = SceneManager.LoadSceneAsync(
            "_Main",
            LoadSceneMode.Additive
        );
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }

        loadingSlider.value = 1f;

        op.allowSceneActivation = true;

        yield return null;

        // Fade OUT setelah scene aktif
        LeanTween.alphaCanvas(loadingCanvasGroup, 0f, 0.5f);
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
