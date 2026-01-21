using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LandingController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private UIManager uiManager;

    [Header("UI Elements")]
    [SerializeField] private GameObject headerTexts;
    [SerializeField] private VideoPlayer landingVideoBackground;
    [SerializeField] private CanvasGroup clickToStartText;


    [Header("Video Clips")]
    //[SerializeField] private VideoClip landingVideoClip;
    //[SerializeField] private VideoClip transitionVideoClip;
    //[SerializeField] private VideoClip homeTransitionVideoClip;

    [SerializeField] private string landingVideoClip;
    [SerializeField] private string transitionVideoClip;
    [SerializeField] private string homeTransitionVideoClip;

    [Header("video errors panel")]
    [SerializeField] private CanvasGroup videoErrorPanel;
    [SerializeField] private TMP_Text videoErrorText;
    [SerializeField] private Button defaultVideoButton;
    [SerializeField] private Button openVideoFolderButton;
    [SerializeField] private Button closeVideoErrorPanelButton;
    [SerializeField] private Button HUDOpenVideoPanelButton;
    [SerializeField] private Button HUDOpenButtonConfirmation;
    private int confirmationCount = 0;

    //variables
    private int fadeTweenId = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initlandingpage();
    }

    private void initlandingpage()
    {
        //GameManager.Instance.CheckDefaultVideos();

        //play background video
        // landingVideoBackground.clip = landingVideoClip;
        // landingVideoBackground.Play();
        landingVideoBackground.source = VideoSource.Url;
        SetVideoLink(landingVideoClip, true);

        //play bgm
        SoundManager.Instance.PlaySFX("theEpic");

        // Begin click to start fade loop
        FadeOutButton();

        uiManager.PopOutFade();

        //init video error panel
        videoErrorPanel.alpha = 0;
        videoErrorPanel.interactable = false;
        videoErrorPanel.blocksRaycasts = false;

        string CURRENT_VERSION = Application.version;
        string LAST_VERSION = PlayerPrefs.GetString("LAST_APP_VERSION", "");

        bool isFirstInstall = string.IsNullOrEmpty(LAST_VERSION);
        bool isVersionChanged = LAST_VERSION != CURRENT_VERSION;

        if (isFirstInstall || isVersionChanged)
        {
            StartCoroutine(GameManager.Instance.CreateDefaultIncidentVideos());
            PlayerPrefs.SetString("LAST_APP_VERSION", CURRENT_VERSION);
            PlayerPrefs.Save();
            Debug.Log("First install or version changed. Created default incident videos.");
        }
        else
        {
            CheckVideoIncident();
        }

        defaultVideoButton.onClick.AddListener(() =>
        {
            StartCoroutine(GameManager.Instance.CreateDefaultIncidentVideos());
            LeanTween.alphaCanvas(videoErrorPanel, 0, 0.5f).setOnComplete(() =>
            {
                videoErrorPanel.interactable = false;
                videoErrorPanel.blocksRaycasts = false;

                HUDOpenVideoPanelButton.gameObject.SetActive(false);
                HUDOpenButtonConfirmation.gameObject.SetActive(true);
            });
        });

        openVideoFolderButton.onClick.AddListener(() =>
        {
            GameManager.Instance.OpenVideoFolder();

            LeanTween.alphaCanvas(videoErrorPanel, 0, 0.5f).setOnComplete(() =>
            {
                videoErrorPanel.interactable = false;
                videoErrorPanel.blocksRaycasts = false;

                HUDOpenVideoPanelButton.gameObject.SetActive(false);
                HUDOpenButtonConfirmation.gameObject.SetActive(true);
            });
        });

        closeVideoErrorPanelButton.onClick.AddListener(() =>
        {
            LeanTween.alphaCanvas(videoErrorPanel, 0, 0.5f).setOnComplete(() =>
            {
                videoErrorPanel.interactable = false;
                videoErrorPanel.blocksRaycasts = false;

                HUDOpenVideoPanelButton.gameObject.SetActive(false);
                HUDOpenButtonConfirmation.gameObject.SetActive(true);
            });
        });

        // HUDOpenVideoPanelButton.onClick.AddListener(() =>
        // {
        //     GameManager.Instance.CheckDefaultVideos();
        //     if (GameManager.Instance.isVideoHaveError)
        //     {
        //         SetErrorVideoText();
        //     }
        //     else
        //     {
        //         videoErrorText.text = "<color=white>No video errors found.";
        //     }
        //     LeanTween.alphaCanvas(videoErrorPanel, 1, 0.5f).setOnComplete(() =>
        //     {
        //         videoErrorPanel.interactable = true;
        //         videoErrorPanel.blocksRaycasts = true;

        //         confirmationCount = 0;
        //     });
        // });

        HUDOpenVideoPanelButton.onClick.AddListener(() =>
        {
            GameManager.Instance.OpenVideoFolder();

            HUDOpenVideoPanelButton.gameObject.SetActive(false);
            HUDOpenButtonConfirmation.gameObject.SetActive(true);
            confirmationCount = 0;
        });

        GameManager.Instance.CheckDefaultVideos();
    }
    #region Video Callbacks
    public void OnClickToStart()
    {
        // Stop the button fade loop
        LeanTween.cancel(fadeTweenId);

        uiManager.initUIManager();
        SoundManager.Instance.PlaySFX("button_click");
        GameManager.Instance.InitializeCampusEventData();

        // Play transition video
        LeanTween.scale(headerTexts, Vector3.zero, 0.5f).setOnComplete(() =>
        {
            // landingVideoBackground.clip = transitionVideoClip;
            // landingVideoBackground.Play();
            SetVideoLink(transitionVideoClip, false);
            landingVideoBackground.loopPointReached += OnTransitionVideoEnd;
        });
    }

    private void OnTransitionVideoEnd(VideoPlayer vp)
    {
        // Unsubscribe so it doesn't trigger multiple times
        vp.loopPointReached -= OnTransitionVideoEnd;
        // landingVideoBackground.clip = homeTransitionVideoClip;
        // landingVideoBackground.Play();
        SetVideoLink(homeTransitionVideoClip, false);
        landingVideoBackground.loopPointReached += OnHomeTransitionVideoEnd;

        uiManager.loadDashboardScene();
    }

    private void OnHomeTransitionVideoEnd(VideoPlayer vp)
    {
        // Unsubscribe so it doesn't trigger multiple times
        vp.loopPointReached -= OnHomeTransitionVideoEnd;
        landingVideoBackground.Stop();
        // Show Dashboard UI
        uiManager.ShowDashboard();
    }

    #endregion

    #region Fade Button Methods
    private void FadeInButton()
    {
        fadeTweenId = LeanTween.value(gameObject, 0f, 1f, 1f)
            .setOnUpdate((float val) => clickToStartText.alpha = val)
            .setOnComplete(FadeOutButton)
            .id;
    }

    private void FadeOutButton()
    {
        fadeTweenId = LeanTween.value(gameObject, 1f, 0f, 1f)
            .setOnUpdate((float val) => clickToStartText.alpha = val)
            .setOnComplete(FadeInButton)
            .id;
    }
    #endregion

    #region Video Error Methods

    public void ActivatedHUDButton()
    {
        GameManager.Instance.OpenVideoFolder();
    }
    public void SetVideoToDefault()
    {
        StartCoroutine(GameManager.Instance.CreateDefaultIncidentVideos());
    }
    void SetErrorVideoText()
    {
        videoErrorText.text = "The following video errors were found:\n";
        foreach (string error in GameManager.Instance.videoErrors)
        {
            videoErrorText.text += "- " + error + "\n";
        }
    }

    public void CheckVideoIncident()
    {
        if (GameManager.Instance.isVideoHaveError)
        {
            SetErrorVideoText();
            LeanTween.alphaCanvas(videoErrorPanel, 1, 0.5f).setOnComplete(() =>
            {
                videoErrorPanel.interactable = true;
                videoErrorPanel.blocksRaycasts = true;
            });
        }
        else
        {
            videoErrorText.text = "<color=white>No video errors found.";
        }
    }
    #endregion

    private void SetVideoLink(string videoName, bool isLooping)
    {
        string path = Path.Combine(
                Application.streamingAssetsPath,
                "Landing",
                videoName + ".mp4"
            );
        string fileUrl = "file://" + path.Replace("\\", "/");

        if (File.Exists(path))
        {
            landingVideoBackground.url = fileUrl;

            landingVideoBackground.isLooping = isLooping;
            landingVideoBackground.Prepare();
            landingVideoBackground.prepareCompleted += (VideoPlayer source) => source.Play();
        }
        else
        {
            Debug.LogError("Video file not found: " + path + "\nformated url: " + fileUrl);
        }
    }
}
