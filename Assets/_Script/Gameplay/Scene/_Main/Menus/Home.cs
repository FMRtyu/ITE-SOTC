using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Home : _MenuState
{
    [Header("UI Elements")]
    [SerializeField] private Sprite menuBG;
    //[SerializeField] private Animator PieChartAnimator;
    [SerializeField] private CanvasGroup[] alertSummaryDatas;
    [SerializeField] private Button alertSummaryButtonData1;
    [SerializeField] private Button alertSummaryButtonData2;
    [SerializeField] private Button alertSummaryButtonData3;
    [SerializeField] private Sprite alertActivateCarousel;
    [SerializeField] private Sprite alertDeactivateCarousel;

    [Header("Campus Event Panel")]
    [SerializeField] private GameObject campusContentPrefab;
    [SerializeField] private GameObject campusContentParent;
    private List<GameObject> campusEventContents = new List<GameObject>();
    bool isInit = false;

    int alertTweenId = -1;
    //Specific for this state
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.Home;
    }

    void Start()
    {
        if (!uiManager.skipLanding)
            HideAllChildren();

        initHome();
    }

    void OnEnable()
    {
        if (uiManager != null)
        {
            uiManager.ChangeBackground(menuBG);
            uiManager.ShowBlackBG(0);
        }

        //PieChartAnimator.SetBool("Init", isInit);
    }

    void OnDisable()
    {
        if (!isInit)
        {
            isInit = true;
        }
    }

    private void HideAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void initHome()
    {
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("button_click");
            });
        }
        ActivateDataChart(0);

        alertSummaryButtonData1.onClick.AddListener(() =>
        {
            ActivateDataChart(0);
            alertSummaryButtonData1.GetComponent<Image>().sprite = alertActivateCarousel;
            alertSummaryButtonData1.interactable = false;

            alertSummaryButtonData2.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertSummaryButtonData2.interactable = true;
            alertSummaryButtonData3.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertSummaryButtonData3.interactable = true;

            SoundManager.Instance.PlaySFX("button_click");
        });
        alertSummaryButtonData2.onClick.AddListener(() =>
        {
            ActivateDataChart(1);
            alertSummaryButtonData2.GetComponent<Image>().sprite = alertActivateCarousel;
            alertSummaryButtonData2.interactable = false;

            alertSummaryButtonData1.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertSummaryButtonData1.interactable = true;
            alertSummaryButtonData3.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertSummaryButtonData3.interactable = true;
            SoundManager.Instance.PlaySFX("button_click");
        });

        alertSummaryButtonData3.onClick.AddListener(() =>
        {
            ActivateDataChart(2);
            alertSummaryButtonData3.GetComponent<Image>().sprite = alertActivateCarousel;
            alertSummaryButtonData3.interactable = false;

            alertSummaryButtonData1.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertSummaryButtonData1.interactable = true;
            alertSummaryButtonData2.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertSummaryButtonData2.interactable = true;
            SoundManager.Instance.PlaySFX("button_click");
        });
        
    }

    public void ShowChildren()
    {
        // Enable all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        uiManager.ChangeBackground(menuBG);
    }

    public void SetCampusEventData()
    {
        foreach (GameObject eventPanel in campusEventContents)
        {
            Destroy(eventPanel);
        }
        campusEventContents.Clear();

        foreach (CampusEventData Data in GameManager.Instance.CampusEvents)
        {
            GameObject temp = Instantiate(campusContentPrefab, campusContentParent.transform);

            temp.GetComponent<EventInfo>().SetEventInfo(Data.@event, Data.venue, Data.time, Data.crowd_size, Data.current_status);

            campusEventContents.Add(temp);
        }
    }

    public void ActivateDataChart(int index)
    {
        LeanTween.cancel(alertTweenId);
        if(index < 0 || index >= alertSummaryDatas.Length)
            return;
        foreach (CanvasGroup data in alertSummaryDatas)
        {
            if(data.alpha > 0)
                alertTweenId = LeanTween.alphaCanvas(data, 0f, 1.5f).setEase(LeanTweenType.easeInOutQuad).id;
        }
        alertTweenId = LeanTween.alphaCanvas(alertSummaryDatas[index], 1f, 1.5f).setEase(LeanTweenType.easeInOutQuad).setDelay(1.5f).id;
    }
}
