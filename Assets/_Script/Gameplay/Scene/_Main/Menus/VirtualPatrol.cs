using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VirtualPatrol : _MenuState
{
    [Header("Video elements")]
    [SerializeField] private VideoPlayer fenceBTNOne;
    [SerializeField] private VideoPlayer fenceBTNTwo;
    [SerializeField] private VideoPlayer PopupVideoPlayer;

    [SerializeField] private CanvasGroup blackBGOverlay;

    [SerializeField] private float blackBGOverlayeDuration = 0.5f;
    [SerializeField] private float blackBGOverlayeDelay = 0.5f;

    [Header("video clips")]
    [SerializeField] private VideoClip[] popupVideoClip;
    [SerializeField] private IntrusionVideo[] intrusionData;

    [Header("UI Element")]
    [SerializeField] private CanvasGroup pinPoint;
    [SerializeField] private CanvasGroup virtualBackgroundGroup;

    [SerializeField] private Button[] staffBTNS;

    //variables
    bool isPlayingPopup;
    bool intrusionInProgress;

    IntrusionVideo currentIntrusionVideo;
    CancellationTokenSource storyboardCTS;
    CanvasGroup currentStaffBTNBG;

    int currentIndex = -1;
    int blackBGOverlayId = -1;

    int virtualBackgroundTweenId = -1;
    int pinPointTweenId = -1;

    CanvasGroup fencePopupVideoCanvasGroup;

    void Awake()
    {
        initVirtualPatrol();
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

        VideoToggle(fenceBTNOne, true);
        VideoToggle(fenceBTNTwo, true);

        EnableSecurityBTN(false);
    }

    void OnDisable()
    {
        StopCamera();

        LeanTween.cancel(virtualBackgroundTweenId);
        LeanTween.cancel(pinPointTweenId);

        virtualBackgroundTweenId = -1;
        pinPointTweenId = -1;

        VideoToggle(fenceBTNOne, false);
        VideoToggle(fenceBTNTwo, false);

        blackBGOverlay.alpha = 0f;

        fenceBTNOne.SetDirectAudioMute(0, true);
        fenceBTNTwo.SetDirectAudioMute(0, true);

        pinPointTweenId = LeanTween.alphaCanvas(pinPoint, 0f, 0.5f)
            .setOnComplete(() =>
            {
                virtualBackgroundTweenId = LeanTween.alphaCanvas(virtualBackgroundGroup, 0f, 0.5f).id;
            }).id;
    }

    void initVirtualPatrol()
    {
        fencePopupVideoCanvasGroup = PopupVideoPlayer.GetComponent<CanvasGroup>();

        fencePopupVideoCanvasGroup.alpha = 0f;
        fencePopupVideoCanvasGroup.interactable = false;
        fencePopupVideoCanvasGroup.blocksRaycasts = false;

        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("button_click");
            });
        }

        int tempIndex = 0;
        foreach (Button btn in staffBTNS)
        {
            btn.interactable = false;
            
            btn.onClick.AddListener(() => SendSecurity(tempIndex,btn));
            tempIndex++;
        }
    }

    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);
        state = MenuState.VirtualPatrol;
    }

    public void PlayPopupVideo(int index)
    {
        if (intrusionInProgress)
            return;

        CancelStoryboardIfRunning();

        foreach (Button btn in staffBTNS)
            btn.interactable = false;

        PopupVideoPlayer.isLooping = true;

        if (index != currentIndex)
        {
            if (!isPlayingPopup)
                PlayPopupCamera(index);
            else
                SwitchCamera(index);
        }
        else
        {
            StopCamera();
        }
    }

    public void PlayPopupScenario(string scenarioName)
    {
        if (intrusionInProgress)
            return;

        CancelStoryboardIfRunning();

        foreach (IntrusionVideo data in intrusionData)
        {
            if (data.scenarioName == scenarioName)
            {
                currentIntrusionVideo = data;
                PopupVideoPlayer.isLooping = false;

                PlayPopupCamera(-1, data);
                return;
            }
        }

        Debug.LogWarning($"Scenario '{scenarioName}' not found.");
    }

    public void SendSecurity(int index, Button staffBTN)
    {
        if (intrusionInProgress)
            return;
        intrusionInProgress = true;

        staffBTN.interactable = false;
        currentStaffBTNBG = staffBTN.transform.Find("Background").GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(currentStaffBTNBG, 1f, 0.5f);

        StartStoryboardSequence();
    }

    // ============================================================
    // INTERNAL VIDEO LOGIC
    // ============================================================

    private void VideoToggle(VideoPlayer vp, bool play)
    {
        if (play) vp.Play();
        else vp.Pause();
    }

    private void SwitchCamera(int index)
    {
        PopupVideoPlayer.Stop();

        LeanTween.alphaCanvas(fencePopupVideoCanvasGroup, 0f, 0.3f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                PopupVideoPlayer.clip = popupVideoClip[index];
                currentIndex = index;
                PopupVideoPlayer.Play();

                LeanTween.alphaCanvas(fencePopupVideoCanvasGroup, 1f, 0.3f)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() =>
                    {
                        fencePopupVideoCanvasGroup.interactable = true;
                        fencePopupVideoCanvasGroup.blocksRaycasts = true;
                    });
            });
    }

    private void PlayPopupCamera(int index = -1, IntrusionVideo intrusionVideo = null)
    {
        ShowPinPoint(false);

        if (intrusionVideo != null)
        {
            PopupVideoPlayer.clip = intrusionVideo.opening;
            PopupVideoPlayer.loopPointReached += OpeningFinished;
        }
        else
        {
            intrusionInProgress = false;
            PopupVideoPlayer.clip = popupVideoClip[index];
            currentIndex = index;
        }

        PopupVideoPlayer.Play();
        isPlayingPopup = true;

        if (blackBGOverlay.alpha < 1f)
            blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 1f, blackBGOverlayeDuration)
                .setEase(LeanTweenType.easeInOutSine).id;

        LeanTween.alphaCanvas(fencePopupVideoCanvasGroup, 1f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fencePopupVideoCanvasGroup.interactable = true;
                fencePopupVideoCanvasGroup.blocksRaycasts = true;
            });
    }

    private void OpeningFinished(VideoPlayer vp)
    {
        PopupVideoPlayer.loopPointReached -= OpeningFinished;

        foreach (Button btn in staffBTNS)
            btn.interactable = true;
    }

    // ============================================================
    // STORYBOARD (async)
    // ============================================================

    private void StartStoryboardSequence()
    {
        CancelStoryboardIfRunning();

        storyboardCTS = new CancellationTokenSource();
        RunStoryboardAsync(currentIntrusionVideo, storyboardCTS.Token);
    }

    private async void RunStoryboardAsync(IntrusionVideo video, CancellationToken ct)
    {
        try
        {
            foreach (var clip in video.storyBoard)
            {
                ct.ThrowIfCancellationRequested();

                PopupVideoPlayer.clip = clip;
                PopupVideoPlayer.Play();

                await VideoAwaiter.WaitForEnd(PopupVideoPlayer);
            }

            StopCamera();
        }
        catch { }
    }

    private void CancelStoryboardIfRunning()
    {
        if (storyboardCTS != null)
        {
            storyboardCTS.Cancel();
            storyboardCTS.Dispose();
            storyboardCTS = null;
        }
    }

    // ============================================================
    private void StopCamera()
    {
        CancelStoryboardIfRunning();

        PopupVideoPlayer.Stop();

        LeanTween.alphaCanvas(fencePopupVideoCanvasGroup, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                if (dashboardController.activeState.state == MenuState.VirtualPatrol)
                    ShowPinPoint(true);

                fencePopupVideoCanvasGroup.interactable = false;
                fencePopupVideoCanvasGroup.blocksRaycasts = false;

                isPlayingPopup = false;

                if (intrusionInProgress)
                {
                    LeanTween.alphaCanvas(currentStaffBTNBG, 0f, 0.5f);
                    currentStaffBTNBG = null;
                    EnableSecurityBTN(false);
                    intrusionInProgress = false;
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
        foreach(Button staffBTN in staffBTNS)
        {
            staffBTN.interactable = newCondition;
        }
    }
}
