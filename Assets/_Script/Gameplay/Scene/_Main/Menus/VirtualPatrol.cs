using NUnit.Framework;
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
    [SerializeField] private RenderTexture fenceCameraOneTexture;
    [SerializeField] private RenderTexture fenceCameraTwoTexture;

    [Header("UI Element")]
    [SerializeField] private CanvasGroup pinPoint;
    [SerializeField] private CanvasGroup virtualBackgroundGroup;

    //video player settings
    private CanvasGroup fencePopupVideoCanvasGroup;

    //variables
    bool isPlayingPopup;

    int currentIndex = -1;
    int blackBGOverlayId = -1;

    int virtualBackgroundTweenId = -1;
    int pinPointTweenId = -1;


    void Awake()
    {
        initVirtualPatrol();
    }

    void OnEnable()
    {
        // Cancel tweens safely
        LeanTween.cancel(virtualBackgroundTweenId);
        LeanTween.cancel(pinPointTweenId);

        blackBGOverlay.alpha = 0f;

        // Store tween ID for virtualBackgroundGroup
        virtualBackgroundTweenId = LeanTween.alphaCanvas(virtualBackgroundGroup, 1f, 0.5f).setDelay(blackBGOverlayeDelay).setOnComplete(() =>
        {
            // Store tween ID for pinPoint
            pinPointTweenId = LeanTween.alphaCanvas(pinPoint, 1f, 0.5f).id;
        }).id;

        VideoToggle(fenceBTNOne, true);
        VideoToggle(fenceBTNTwo, true);
    }

    void OnDisable()
    {
        StopCamera();
        // Cancel tweens safely
        LeanTween.cancel(virtualBackgroundTweenId);
        LeanTween.cancel(pinPointTweenId);

        virtualBackgroundTweenId = -1;
        pinPointTweenId = -1;

        VideoToggle(fenceBTNOne, false);
        VideoToggle(fenceBTNTwo, false);

        // Immediately hide the overlay so it doesn't remain visible
        blackBGOverlay.alpha = 0f;

        fenceBTNOne.SetDirectAudioMute(0, true);
        fenceBTNTwo.SetDirectAudioMute(0, true);

        // Fade out tweens with IDs again
        pinPointTweenId = LeanTween.alphaCanvas(pinPoint, 0f, 0.5f).setOnComplete(() =>
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

    }

    public void PlayPopupVideo(int index)
    {
        if (index != currentIndex)
            if (!isPlayingPopup)
                PlayPopupCamera(index);
            else
                SwitchCamera(index);
        else
            StopCamera();

    }
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.VirtualPatrol;
    }

    private void VideoToggle(VideoPlayer vp, bool play)
    {
        if (play)
        {
            vp.Play();
        }
        else
        {
            vp.Pause();
        }
    }



    // === Smooth transition between two cameras ===
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
    private void PlayPopupCamera(int index)
    {
        ShowPinPoint(false);
        PopupVideoPlayer.clip = popupVideoClip[index];
        PopupVideoPlayer.Play();

        isPlayingPopup = true;
        currentIndex = index;

        if (blackBGOverlay.alpha < 1f)
            blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 1f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine).id;

        LeanTween.alphaCanvas(fencePopupVideoCanvasGroup, 1f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fencePopupVideoCanvasGroup.interactable = true;
                fencePopupVideoCanvasGroup.blocksRaycasts = true;
                PopupVideoPlayer.Play();
            });

    }

    private void StopCamera()
    {
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
            LeanTween.alphaCanvas(pinPoint, 0f, 1f).setOnComplete(() =>
            {
                pinPoint.interactable = false;
                pinPoint.blocksRaycasts = false;
            });
        }

    }
}
