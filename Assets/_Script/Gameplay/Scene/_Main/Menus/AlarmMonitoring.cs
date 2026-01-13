using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlarmMonitoring : _MenuState
{
    [SerializeField] private Sprite menuBG;
    [Header("Alert Summary Elements")]
    [SerializeField] private CanvasGroup[] alertContents;
    [SerializeField] private TMP_Text alarmAlertsText;
    private CanvasGroup alarmAlertsCanvasGroup;
    int alarmAlertsTweenId = -1;
    [SerializeField] private Button alertButtonContent1;
    [SerializeField] private Button alertButtonContent2;
    [SerializeField] private Button alertButtonContent3;
    [SerializeField] private Sprite alertActivateCarousel;
    [SerializeField] private Sprite alertDeactivateCarousel;
    int alertTweenId = -1;

    void OnEnable()
    {
        if (uiManager != null)
        {
            uiManager.ChangeBackground(menuBG);
            uiManager.ShowBlackBG(3);

            LeanTween.alphaCanvas(uiManager.alarmMonitoringCG, 1f, 1f).setEase(LeanTweenType.easeInOutQuad).setDelay(1f);
        }
    }

    void OnDisable()
    {
        uiManager.alarmMonitoringCG.alpha = 0f;
    }

    void Start()
    {
        alertButtonContent1.onClick.AddListener(() =>
        {
            ActivateDataChart(0, "03");
            alertButtonContent1.GetComponent<Image>().sprite = alertActivateCarousel;
            alertButtonContent1.interactable = false;

            alertButtonContent2.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertButtonContent2.interactable = true;
            alertButtonContent3.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertButtonContent3.interactable = true;
            SoundManager.Instance.PlaySFX("button_click");
        });

        alertButtonContent2.onClick.AddListener(() =>
        {
            ActivateDataChart(1, "04");
            alertButtonContent2.GetComponent<Image>().sprite = alertActivateCarousel;
            alertButtonContent2.interactable = false;

            alertButtonContent1.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertButtonContent1.interactable = true;
            alertButtonContent3.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertButtonContent3.interactable = true;
            SoundManager.Instance.PlaySFX("button_click");
        });

        alertButtonContent3.onClick.AddListener(() =>
        {
            ActivateDataChart(2, "01");
            alertButtonContent3.GetComponent<Image>().sprite = alertActivateCarousel;
            alertButtonContent3.interactable = false;

            alertButtonContent1.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertButtonContent1.interactable = true;
            alertButtonContent2.GetComponent<Image>().sprite = alertDeactivateCarousel;
            alertButtonContent2.interactable = true;
            SoundManager.Instance.PlaySFX("button_click");
        });

        alarmAlertsCanvasGroup = alarmAlertsText.GetComponent<CanvasGroup>();
    }
    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);

        state = MenuState.AlarmMonitoring;
    }

    public void ActivateDataChart(int index, string alertCount)
    {
        LeanTween.cancel(alertTweenId);
        LeanTween.cancel(alarmAlertsTweenId);
        if(index < 0 || index >= alertContents.Length)
            return;
        alarmAlertsTweenId = LeanTween.alphaCanvas(alarmAlertsCanvasGroup, 0f, 1.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            alarmAlertsText.text = alertCount;
        }).id;
        foreach (CanvasGroup data in alertContents)
        {
            if(data.alpha > 0)
                alertTweenId = LeanTween.alphaCanvas(data, 0f, 1.5f).setEase(LeanTweenType.easeInOutQuad).id;
        }

        alertTweenId = LeanTween.alphaCanvas(alertContents[index], 1f, 1.5f).setEase(LeanTweenType.easeInOutQuad).setDelay(1.5f).id;
        alarmAlertsTweenId = LeanTween.alphaCanvas(alarmAlertsCanvasGroup, 1f, 1.5f).setEase(LeanTweenType.easeInOutQuad).setDelay(1.5f).id;
    }
}
