using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private bool isShowing = false;

    private int changingID = -1;

    private bool isplayingBGM = false;

    private List<GameObject> navGroup = new List<GameObject>();

    [Header("debug")]
    [SerializeField] private bool skipLanding = false;

    [SerializeField] private GameObject designReference;

    //var
    private DateTime today;

    private Sprite defaultBG;

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

        designReference.SetActive(false);

        defaultBG = backgroundCG.GetComponent<UnityEngine.UI.Image>().sprite;
    }

    private void initUIElement()
    {
        today = DateTime.Now;
        landingCanvasGroup = landingPage.GetComponent<CanvasGroup>();
        dashboardCanvasGroup = dashboardPage.GetComponent<CanvasGroup>();
        // Format: 6th October 2025 Friday
        string formatted = today.ToString("d MMMM yyyy dddd");
        // Add "st / nd / rd / th"
        formatted = AddOrdinal(today.Day) + today.ToString(" MMMM yyyy dddd");

        dateText.text = formatted;

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
        StartCoroutine(UpdateTime());

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
        if(isChangingMenu || dashboardPage.CheckScenarioInProgress())
        {
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
            dashboardPage.SetActiveState(selectedMenu);
            activeBTN(selectedNavButton);
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

            SoundManager.Instance.SetVolume(0.1f, "theEpic", 1f);
            isplayingBGM = true;
        });

    }

    public void ChangeBackground(Sprite newBG)
    {
        LeanTween.cancel(changingID);

        Image bgImage = backgroundCG.GetComponent<Image>();

        if (newBG == bgImage.sprite)
            return;
        changingID = LeanTween.alphaCanvas(backgroundCG, 0f, 1f).setOnComplete(() =>
        {
            if (newBG == null)

                bgImage.sprite = defaultBG;
            else
                bgImage.sprite = newBG;
            changingID = LeanTween.alphaCanvas(backgroundCG, 1f, 1f).id;
        }).id;
    }
    string AddOrdinal(int day)
    {
        if (day % 10 == 1 && day != 11) return day + "st";
        if (day % 10 == 2 && day != 12) return day + "nd";
        if (day % 10 == 3 && day != 13) return day + "rd";
        return day + "th";
    }

    public void toggleBGM()
    {
        if (!isplayingBGM)
        {
            SoundManager.Instance.SetVolume(0.1f, "theEpic", 1f);
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
