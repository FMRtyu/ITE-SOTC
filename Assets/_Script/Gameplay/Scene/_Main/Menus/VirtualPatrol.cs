using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VirtualPatrol : _MenuState
{
    [Header("Video elements")]
    [SerializeField] private VideoPlayer PopupVideoPlayer;
    [SerializeField] private VideoPlayer FullScreenVP;
    [SerializeField] private VideoPlayer droneVP;

    [SerializeField] private TMP_Text scenarioTitleTXT;
    [SerializeField] private CanvasGroup blackBGOverlay;

    [SerializeField] private float blackBGOverlayeDuration = 0.5f;
    [SerializeField] private float blackBGOverlayeDelay = 0.5f;
    private RenderTexture popupTextureDefault;

    [Header("video clips")]
    //[SerializeField] private VideoClip[] CCTVFeedClips;
    [SerializeField] private string[] CCTVFeedClips;
    //[SerializeField] private Sprite[] popupPlaceholder;
    [SerializeField] private IntrusionVideo[] intrusionData;

    [Header("UI Element")]
    [SerializeField] private CanvasGroup pinPoint;
    [SerializeField] private CanvasGroup virtualBackgroundGroup;
    [SerializeField] private Button[] staffBTNS;

    [Header("Incident elements")]
    [SerializeField] private GameObject intrusionPrefab;
    [SerializeField] private Transform intrusionParent;
    private List<GameObject> intrusionInstances = new List<GameObject>();
    List<int> selectedStaffIndex = new List<int>();

    //variables
    //popup video
    int currentIndex = -1;
    int blackBGOverlayId = -1;
    bool isPlayingPopup;

    //intrusion video
    public bool intrusionInProgress { get; private set; } = false;
    IntrusionVideo currentIntrusionVideo;
    CancellationTokenSource storyboardCTS;
    List<CanvasGroup> currentStaffBTNBG = new List<CanvasGroup>();
    List<Button> selectedStaffBTN = new List<Button>();

    string[] letters = { "A", "B", "C", "D", "E" };

    //tweens
    int virtualBackgroundTweenId = -1;
    int pinPointTweenId = -1;

    CanvasGroup PopupVideoCG;
    CanvasGroup fullscreenVideoCG;

    void Awake()
    {
        initVirtualPatrolAwake();
    }

    void Start()
    {
        initIncidentVideo();
    }

    void OnEnable()
    {
        LeanTween.cancel(virtualBackgroundTweenId);
        LeanTween.cancel(pinPointTweenId);

        blackBGOverlay.alpha = 0f;

        virtualBackgroundTweenId = LeanTween.alphaCanvas(virtualBackgroundGroup, 1f, 0.5f)
            .setDelay(blackBGOverlayeDelay)
            .setOnComplete(() =>
            {
                pinPointTweenId = LeanTween.alphaCanvas(pinPoint, 1f, 0.5f).id;
            }).id;

        EnableSecurityBTN(false);

        if (uiManager != null)
            uiManager.ShowBlackBG(2);
    }

    void OnDisable()
    {
        //CancelStoryboardIfRunning();
        StopCamera();

        LeanTween.cancel(virtualBackgroundTweenId);
        LeanTween.cancel(pinPointTweenId);

        virtualBackgroundTweenId = -1;
        pinPointTweenId = -1;

        blackBGOverlay.alpha = 0f;

        pinPointTweenId = LeanTween.alphaCanvas(pinPoint, 0f, 1f)
            .setOnComplete(() =>
            {
                virtualBackgroundTweenId = LeanTween.alphaCanvas(virtualBackgroundGroup, 0f, 1f).id;
            }).id;
    }

    void initVirtualPatrolAwake()
    {
        uiManager = GameManager.Instance.uIManager;
        PopupVideoPlayer = uiManager.popupVideoPlayer;
        pinPoint = uiManager.pinPointCG;
        virtualBackgroundGroup = uiManager.virtualBGCG;
        blackBGOverlay = uiManager.popupBlackBGCG;

        PopupVideoCG = PopupVideoPlayer.GetComponent<CanvasGroup>();

        PopupVideoCG.alpha = 0f;
        PopupVideoCG.interactable = false;
        PopupVideoCG.blocksRaycasts = false;

        fullscreenVideoCG = FullScreenVP.GetComponent<CanvasGroup>();

        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // Skip staff buttons; they will have their own custom rules
            if (System.Array.Exists(staffBTNS, x => x == button))
                continue;

            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("button_click");
            });
        }

        // int tempIndex = 1;
        // foreach (Button btn in staffBTNS)
        // {
        //     btn.interactable = false;

        //     int indexCopy = tempIndex;
        //     Animator btnAnimator = btn.GetComponent<Animator>();

        //     btn.onClick.AddListener(() =>
        //     {
        //         SendSecurity(indexCopy, btn);

        //         // Only play if not in progress
        //         if (btnAnimator == null || !btnAnimator.GetBool("inProgress"))
        //             SoundManager.Instance.PlaySFX("button_click");
        //     });

        //     tempIndex++;
        // }
    }

    void initIncidentVideo()
    {
        SetVideoLinkstreamingAssetsPath(CCTVFeedClips[4], true, droneVP);
        popupTextureDefault = PopupVideoPlayer.targetTexture;

        int tempIntrusionIndex = 1;
        foreach (IntrusionVideo data in intrusionData)
        {
            GameObject intrusionBTNInstance = Instantiate(intrusionPrefab, intrusionParent);

            var vp = intrusionBTNInstance.GetComponentInChildren<VideoPlayer>();
            var raw = intrusionBTNInstance.GetComponentInChildren<RawImage>();

            var rt = new RenderTexture(1920, 1080, 0);

            vp.targetTexture = rt;
            raw.texture = rt;

            vp.source = VideoSource.Url;
            string path = Path.Combine(
                Application.persistentDataPath,
                "videos",
                data.moduleFolder,
                data.openingURL + ".mp4"
            );

            string fileUrl = "file://" + path.Replace("\\", "/");

            vp.url = fileUrl;

            vp.isLooping = true;
            vp.Prepare();
            vp.prepareCompleted += (VideoPlayer source) => source.Play();

            intrusionBTNInstance.transform.Find("CameraTXT").GetComponent<TMP_Text>().text = "CAM " + tempIntrusionIndex;
            intrusionBTNInstance.transform.Find("Group/GroupTXT").GetComponent<TMP_Text>().text = letters[tempIntrusionIndex - 1];
            intrusionBTNInstance.transform.Find("LogoBG/Logo").GetComponent<Image>().sprite = data.icon;

            intrusionBTNInstance.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                PlayPopupScenario(data.scenarioName);
                SoundManager.Instance.PlaySFX("button_click");
            });

            intrusionInstances.Add(intrusionBTNInstance);

            tempIntrusionIndex++;
        }
    }

    public override void InitState(DashboardController dashboardController, UIManager uiManager)
    {
        base.InitState(dashboardController, uiManager);
        state = MenuState.VirtualPatrol;
    }

    public void PlayPopupVideo(int index)
    {
        if (intrusionInProgress)
            return;

        //CancelStoryboardIfRunning();

        foreach (Button btn in staffBTNS)
            btn.interactable = false;

        PopupVideoPlayer.isLooping = true;

        if (uiManager.isplayingBGM)
            SoundManager.Instance.SetVolume(0.01f, "theEpic", 1f);

        if (index == currentIndex && isPlayingPopup)
        {
            StopCamera();

            if (uiManager.isplayingBGM)
                SoundManager.Instance.SetVolume(0.02f, "theEpic", 1f);

            return;
        }

        if (!isPlayingPopup)
            PlayPopupCamera(index);
        else
            SwitchCamera(index);
    }

    // public void PlayPopupScenario(string scenarioName)
    // {
    //     if (intrusionInProgress)
    //         return;
    //     SoundManager.Instance.SetVolume(0.05f, "theEpic", 1f);
    //     CancelStoryboardIfRunning();

    //     foreach (IntrusionVideo data in intrusionData)
    //     {
    //         if (data.scenarioName == scenarioName)
    //         {
    //             currentIntrusionVideo = data;
    //             PopupVideoPlayer.isLooping = false;

    //             PlayPopupCamera(-1, data);
    //             return;
    //         }
    //     }

    //     Debug.LogWarning($"Scenario '{scenarioName}' not found.");
    // }

    public void PlayPopupScenario(string scenarioName)
    {
        if (intrusionInProgress)
            return;
        if (uiManager.isplayingBGM)
            SoundManager.Instance.SetVolume(0.01f, "theEpic", 1f);

        GetIntrusionByName(scenarioName);

        scenarioTitleTXT.text = currentIntrusionVideo.scenarioName;

        PlayScenarioVideo(currentIntrusionVideo.moduleFolder, currentIntrusionVideo.openingURL);
        //FullScreenVP.clip = currentIntrusionVideo.opening;
        //FullScreenVP.Play();
        LeanTween.alphaCanvas(fullscreenVideoCG, 1f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullscreenVideoCG.interactable = true;
                fullscreenVideoCG.blocksRaycasts = true;

                //CancelStoryboardIfRunning();
            });
    }

    public void SendSecurityToLocation()
    {
        FullScreenVP.Stop();
        LeanTween.alphaCanvas(fullscreenVideoCG, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullscreenVideoCG.interactable = false;
                fullscreenVideoCG.blocksRaycasts = false;

                if (intrusionInProgress)
                {
                    if (uiManager.isplayingBGM)
                        SoundManager.Instance.SetVolume(0.02f, "theEpic", 1f);
                    intrusionInProgress = false;
                }
                else
                {
                    foreach (int staffIndex in currentIntrusionVideo.correctStaffIndex)
                    {
                        Button tempBTN = staffBTNS[staffIndex];
                        CanvasGroup tempBackground = tempBTN.transform.Find("Background").GetComponent<CanvasGroup>();

                        LeanTween.alphaCanvas(tempBackground, 1f, 0.5f);
                        tempBTN.interactable = false;

                        selectedStaffBTN.Add(tempBTN);
                        currentStaffBTNBG.Add(tempBackground);
                        selectedStaffIndex.Add(staffIndex);

                    }

                    //StartStoryboardSequence();
                    intrusionInProgress = true;

                    PlayPopupCamera(-1, currentIntrusionVideo);
                }
            });
    }

    // public void SendSecurity(int index, Button staffBTN)
    // {
    //     if (intrusionInProgress)
    //         return;

    //     // Lock button selected
    //     staffBTN.interactable = false;
    //     selectedStaffIndex.Add(index);

    //     // Highlight background
    //     CanvasGroup tempBackground = staffBTN.transform.Find("Background").GetComponent<CanvasGroup>();
    //     LeanTween.alphaCanvas(tempBackground, 1f, 0.5f);

    //     selectedStaffBTN.Add(staffBTN);
    //     currentStaffBTNBG.Add(tempBackground);

    //     assignTXT.text = "ASSIGN NEAREST OFFICER?\n[" + selectedStaffBTN.Count + "/" + currentIntrusionVideo.correctStaffIndex.Length + "]";

    //     // If staff minimum reached, check correctness
    //     if (selectedStaffBTN.Count >= currentIntrusionVideo.correctStaffIndex.Length)
    //     {
    //         intrusionInProgress = true;

    //         // Tutup notifikasi kalau masih ada
    //         //uiManager.HideNotification();

    //         bool isCorrect = CheckStaffCorrect();

    //         if (isCorrect)
    //         {
    //             assignPanelTweenId = LeanTween.alphaCanvas(assignBlue, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
    //             {
    //                 assignBGTweenId = LeanTween.alphaCanvas(assignBG, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
    //                 {
    //                     assignBG.interactable = false;
    //                     assignBG.blocksRaycasts = false;
    //                     assignBlue.interactable = false;
    //                     assignBlue.blocksRaycasts = false;

    //                     StartStoryboardSequence();
    //                 }).id;
    //             }).id;
    //         }
    //         else
    //         {
    //             //uiManager.ShowNotification("Wrong Security Staff Selected!", 2f);

    //             assignRed.interactable = true;
    //             assignRed.blocksRaycasts = true;
    //             assignPanelTweenId = LeanTween.alphaCanvas(assignRed, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
    //             {
    //                 assignPanelTweenId = LeanTween.alphaCanvas(assignRed, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine).setDelay(3f).setOnComplete(() =>
    //                 {
    //                     assignRed.interactable = false;
    //                     assignRed.blocksRaycasts = false;

    //                     ResetStaffSelection();
    //                     assignTXT.text = "ASSIGN NEAREST OFFICER?\n[" + selectedStaffBTN.Count + "/" + currentIntrusionVideo.correctStaffIndex.Length + "]";
    //                     intrusionInProgress = false;
    //                 }).id;
    //             }).id;
    //         }
    //     }
    // }

    private bool CheckStaffCorrect()
    {
        foreach (int correctIndex in currentIntrusionVideo.correctStaffIndex)
        {
            if (!selectedStaffIndex.Contains(correctIndex))
                return false;
        }

        return true;
    }

    private void ResetStaffSelection()
    {
        // Reset animator
        foreach (Button btn in staffBTNS)
            btn.GetComponent<Animator>().SetBool("inProgress", false);

        // Fade out highlight
        foreach (CanvasGroup cg in currentStaffBTNBG)
            LeanTween.alphaCanvas(cg, 0f, 0.5f);

        // Clear list
        selectedStaffBTN.Clear();
        currentStaffBTNBG.Clear();
        selectedStaffIndex.Clear();

        // Enable semua staff button lagi
        EnableSecurityBTN(true);
    }

    // ============================================================
    // INTERNAL VIDEO LOGIC
    // ============================================================

    private void SwitchCamera(int index)
    {
        //placeholder for now
        //PopupVideoPlayer.Stop();

        LeanTween.alphaCanvas(PopupVideoCG, 0f, 0.3f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                // PopupVideoPlayer.clip = CCTVFeedClips[index];
                // currentIndex = index;
                // PopupVideoPlayer.Play();

                if (index != 4)
                {
                    PopupVideoPlayer.GetComponent<RawImage>().texture = popupTextureDefault;
                    SetVideoLinkstreamingAssetsPath(CCTVFeedClips[index], true, PopupVideoPlayer);
                }
                else
                {
                    PopupVideoPlayer.GetComponent<RawImage>().texture = droneVP.targetTexture;
                    PopupVideoPlayer.Play();
                }
                currentIndex = index;

                //placeholder
                //Image tempPlaceholder = PopupVideoPlayer.transform.Find("PlaceholderIMG").GetComponent<Image>();
                //tempPlaceholder.sprite = popupPlaceholder[index];

                LeanTween.alphaCanvas(PopupVideoCG, 1f, 0.3f).setEase(LeanTweenType.easeInOutSine);
            });
    }

    private void PlayPopupCamera(int index = -1, IntrusionVideo intrusionVideo = null)
    {
        ShowPinPoint(false);
        //Image tempPlaceholder = PopupVideoPlayer.transform.Find("PlaceholderIMG").GetComponent<Image>();

        if (intrusionVideo != null)
        {
            PopupVideoPlayer.GetComponent<RawImage>().texture = popupTextureDefault;
            PopupVideoPlayer.source = VideoSource.Url;
            string path = Path.Combine(
                Application.persistentDataPath,
                "videos",
                intrusionVideo.moduleFolder,
                intrusionVideo.approachURL + ".mp4"
            );

            string fileUrl = "file://" + path.Replace("\\", "/");
            PopupVideoPlayer.url = fileUrl;
            PopupVideoPlayer.Prepare();
            PopupVideoPlayer.prepareCompleted += vp => vp.Play();

            PopupVideoPlayer.loopPointReached += OnApproachFinished;

            PopupVideoPlayer.isLooping = false;

            //tempPlaceholder.gameObject.SetActive(false);
            //placeholder for now
            //PopupVideoPlayer.Play();
            isPlayingPopup = true;
        }
        else
        {
            PopupVideoPlayer.source = VideoSource.VideoClip;
            intrusionInProgress = false;
            // PopupVideoPlayer.clip = CCTVFeedClips[index];
            if(index != 4)
            {
                PopupVideoPlayer.GetComponent<RawImage>().texture = popupTextureDefault;
                SetVideoLinkstreamingAssetsPath(CCTVFeedClips[index], true, PopupVideoPlayer);
            }
            else
            {
                PopupVideoPlayer.GetComponent<RawImage>().texture = droneVP.targetTexture;
                PopupVideoPlayer.Play();
            }
            currentIndex = index;
            PopupVideoPlayer.isLooping = true;
            isPlayingPopup = true;

            //placeholder for now
            //tempPlaceholder.gameObject.SetActive(true);
            //tempPlaceholder.sprite = popupPlaceholder[index];
        }

        PopupVideoPlayer.Play();
        if (blackBGOverlay.alpha < 1f)
            blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 1f, blackBGOverlayeDuration)
                .setEase(LeanTweenType.easeInOutSine).id;
        LeanTween.alphaCanvas(PopupVideoCG, 1f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                PopupVideoCG.interactable = true;
                PopupVideoCG.blocksRaycasts = true;
            });
    }

    // private void OpeningFinished(VideoPlayer vp)
    // {
    //     PopupVideoPlayer.loopPointReached -= OpeningFinished;

    //     //uiManager.ShowNotification("Intrusion Detected! Select the correct Security Staff!", 5f);

    //     assignTXT.text = "ASSIGN NEAREST OFFICER?\n[0/" + currentIntrusionVideo.correctStaffIndex.Length + "]";

    //     assignBG.interactable = true;
    //     assignBG.blocksRaycasts = true;
    //     assignBGTweenId = LeanTween.alphaCanvas(assignBG, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
    //     {
    //         LeanTween.alphaCanvas(assignBlue, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine);
    //     }).id;

    //     EnableSecurityBTN(true);
    // }

    private void OnApproachFinished(VideoPlayer vp)
    {
        FullScreenVP.loopPointReached -= OnApproachFinished;

        StopCamera();

        PlayScenarioVideo(currentIntrusionVideo.moduleFolder, currentIntrusionVideo.solutionURL);
        //FullScreenVP.clip = currentIntrusionVideo.solution;
        //FullScreenVP.Play();
        LeanTween.alphaCanvas(fullscreenVideoCG, 1f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullscreenVideoCG.interactable = true;
                fullscreenVideoCG.blocksRaycasts = true;
            });
    }

    // ============================================================
    // STORYBOARD (async)
    // ============================================================

    // private void StartStoryboardSequence()
    // {
    //     CancelStoryboardIfRunning();

    //     foreach (Button btn in staffBTNS)
    //     {
    //         if (btn.interactable)
    //             btn.GetComponent<Animator>().SetBool("inProgress", true);
    //     }

    //     storyboardCTS = new CancellationTokenSource();
    //     RunStoryboardAsync(currentIntrusionVideo, storyboardCTS.Token);
    // }

    // private async void RunStoryboardAsync(IntrusionVideo video, CancellationToken ct)
    // {
    //     try
    //     {
    //         foreach (var clip in video.storyBoard)
    //         {
    //             ct.ThrowIfCancellationRequested();

    //             PopupVideoPlayer.clip = clip;
    //             PopupVideoPlayer.Play();

    //             await VideoAwaiter.WaitForEnd(PopupVideoPlayer);
    //         }
    //         StopCamera();
    //     }
    //     catch { }
    // }

    // private void CancelStoryboardIfRunning()
    // {
    //     if (storyboardCTS != null)
    //     {
    //         storyboardCTS.Cancel();
    //         storyboardCTS.Dispose();
    //         storyboardCTS = null;
    //     }

    //     LeanTween.cancel(assignPanelTweenId);
    //     LeanTween.cancel(assignBGTweenId);

    //     assignBG.alpha = 0f;
    //     assignBlue.alpha = 0f;
    //     assignRed.alpha = 0f;

    //     assignBG.interactable = false;
    //     assignBG.blocksRaycasts = false;
    //     assignBlue.interactable = false;
    //     assignBlue.blocksRaycasts = false;
    //     assignRed.interactable = false;
    //     assignRed.blocksRaycasts = false;
    // }

    private void StopCamera()
    {
        //CancelStoryboardIfRunning();

        PopupVideoPlayer.Stop();

        LeanTween.alphaCanvas(PopupVideoCG, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                if (dashboardController.activeState.state == MenuState.VirtualPatrol)
                    ShowPinPoint(true);

                PopupVideoCG.interactable = false;
                PopupVideoCG.blocksRaycasts = false;

                isPlayingPopup = false;

                if (intrusionInProgress)
                {
                    foreach (Button btn in staffBTNS)
                        btn.GetComponent<Animator>().SetBool("inProgress", false);
                    foreach (CanvasGroup cg in currentStaffBTNBG)
                        LeanTween.alphaCanvas(cg, 0f, 0.5f);

                    selectedStaffBTN.Clear();
                    currentStaffBTNBG.Clear();
                    selectedStaffIndex.Clear();

                    EnableSecurityBTN(false);
                }
                currentIndex = -1;
            });

        blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 0f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine).id;
    }

    void ShowPinPoint(bool show)
    {
        if (show)
        {
            pinPoint.interactable = true;
            pinPoint.blocksRaycasts = true;
            LeanTween.alphaCanvas(pinPoint, 1f, 1f);
        }
        else
        {
            LeanTween.alphaCanvas(pinPoint, 0f, 1f)
                .setOnComplete(() =>
                {
                    pinPoint.interactable = false;
                    pinPoint.blocksRaycasts = false;
                });
        }
    }

    void EnableSecurityBTN(bool newCondition)
    {
        foreach (Button staffBTN in staffBTNS)
        {
            staffBTN.interactable = newCondition;
        }
    }

    void GetIntrusionByName(string scenarioName)
    {
        foreach (IntrusionVideo data in intrusionData)
        {
            if (data.scenarioName == scenarioName)
            {
                currentIntrusionVideo = data;
                return;
            }
        }
    }

    void PlayScenarioVideo(string moduleFolder, string videoKey)
    {
        string fileName = videoKey; // "VideoA" / "VideoB" / "VideoC"

        string path = Path.Combine(
            Application.persistentDataPath,
            "videos",
            currentIntrusionVideo.moduleFolder,
            fileName + ".mp4"
        );

        if (!File.Exists(path))
        {
            Debug.LogError("Video not found: " + path);
            return;
        }

        string fileUrl = "file://" + path.Replace("\\", "/");

        Debug.Log("VIDEO PATH: " + path);
        Debug.Log("VIDEO URL : " + fileUrl);

        FullScreenVP.source = VideoSource.Url;
        FullScreenVP.url = fileUrl;
        FullScreenVP.Prepare();

        FullScreenVP.prepareCompleted -= OnPrepared;
        FullScreenVP.prepareCompleted += OnPrepared;
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }

    private void SetVideoLinkstreamingAssetsPath(string videoName, bool isLooping, VideoPlayer vp)
    {
        string path = Path.Combine(
                Application.streamingAssetsPath,
                "CCTVFeed",
                videoName + ".mp4"
            );
        string fileUrl = "file://" + path.Replace("\\", "/");

        if (File.Exists(path))
        {
            vp.url = fileUrl;

            vp.isLooping = isLooping;
            vp.Prepare();
            vp.prepareCompleted += (VideoPlayer source) => source.Play();
        }
        else
        {
            Debug.LogError("Video file not found: " + path + "\nformated url: " + fileUrl);
        }
    }


}
