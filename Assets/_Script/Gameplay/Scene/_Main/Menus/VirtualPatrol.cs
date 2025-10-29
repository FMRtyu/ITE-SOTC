using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VirtualPatrol : _MenuState
{
    [Header("Video elements")]
    [SerializeField] private VideoPlayer fullscreenVideoPlayer;
    [SerializeField] private RawImage fullBackgroundVideoPlayer;

    [SerializeField] private VideoPlayer fenceIntrusionVideoPlayerOne;
    [SerializeField] private VideoPlayer fenceIntrusionVideoRawImageTwo;

    [SerializeField] private CanvasGroup blackBGOverlay;

    [SerializeField] private float blackBGOverlayeDuration = 0.5f;
    [SerializeField] private float blackBGOverlayeDelay = 0.5f;

    [Header("video clips")]
    [SerializeField] private VideoClip[] fullscreenVideoClip;
    [SerializeField] private RenderTexture fenceCameraOneTexture;
    [SerializeField] private RenderTexture fenceCameraTwoTexture;

    //video player settings
    private CanvasGroup fullscreenVideoCanvasGroup;
    private CanvasGroup fullBackgroundVideoCanvasGroup;

    //variables
    private bool isPlayingCameraOne = false;
    private bool isPlayingCameraTwo = false;

    int blackBGOverlayId = -1;


    void Awake()
    {
        initVirtualPatrol();
    }

    void OnEnable()
    {
        // Cancel any leftover tween before starting a new one
        LeanTween.cancel(blackBGOverlayId);

        blackBGOverlay.alpha = 0f;
        blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 1f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setDelay(blackBGOverlayeDelay)
            .setOnComplete(() =>
            {
                ToggleFenceCameraViewOne();
                SoundManager.Instance.PlaySFX("alarm");
            })
            .id;
    }

    void OnDisable()
    {
        // Cancel the current fade tween if active
        LeanTween.cancel(blackBGOverlayId);
        blackBGOverlayId = -1;

        // Stop camera feeds safely
        if (isPlayingCameraOne)
            StopCamera("one");

        if (isPlayingCameraTwo)
            StopCamera("two");

        // Immediately hide the overlay so it doesn't remain visible
        blackBGOverlay.alpha = 0f;

        SoundManager.Instance.StopLoopedSFX();
    }

    void initVirtualPatrol()
    {
        fullscreenVideoCanvasGroup = fullscreenVideoPlayer.GetComponent<CanvasGroup>();
        fullBackgroundVideoCanvasGroup = fullBackgroundVideoPlayer.GetComponent<CanvasGroup>();

        fullscreenVideoCanvasGroup.alpha = 0f;
        fullscreenVideoCanvasGroup.interactable = false;
        fullscreenVideoCanvasGroup.blocksRaycasts = false;

        fullBackgroundVideoCanvasGroup.alpha = 0f;
        fullBackgroundVideoCanvasGroup.interactable = false;
        fullBackgroundVideoCanvasGroup.blocksRaycasts = false;

        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("button_click");
            });
        }

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

    public void PlayFullscreenVideo(int index)
    {
        if (index < 0 || index >= fullscreenVideoClip.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for fullscreen video clips.");
            return;
        }

        fullscreenVideoPlayer.clip = fullscreenVideoClip[index];
        fullscreenVideoPlayer.Play();
        LeanTween.alphaCanvas(fullscreenVideoCanvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            fullscreenVideoCanvasGroup.interactable = true;
            fullscreenVideoCanvasGroup.blocksRaycasts = true;
        });
    }

    public void StopFullscreenVideo()
    {
        LeanTween.alphaCanvas(fullscreenVideoCanvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            fullscreenVideoPlayer.Stop();
            fullscreenVideoCanvasGroup.interactable = false;
            fullscreenVideoCanvasGroup.blocksRaycasts = false;
        });
    }

    public void ToggleFenceCameraViewOne()
    {
        if (!isPlayingCameraOne)
        {
            if (isPlayingCameraTwo)
                SwitchCamera(fenceCameraOneTexture, "one");
            else
                PlayCamera(fenceCameraOneTexture, "one");
        }
        else
        {
            StopCamera("one");
        }
    }

    public void ToggleFenceCameraViewTwo()
    {
        if (!isPlayingCameraTwo)
        {
            if (isPlayingCameraOne)
                SwitchCamera(fenceCameraTwoTexture, "two");
            else
                PlayCamera(fenceCameraTwoTexture, "two");
        }
        else
        {
            StopCamera("two");
        }
    }

    // === Smooth transition between two cameras ===
    private void SwitchCamera(RenderTexture toTexture, string toCam)
    {
        LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 0f, 0.3f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullBackgroundVideoPlayer.texture = toTexture;

                LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 1f, 0.3f)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() =>
                    {
                        fullBackgroundVideoCanvasGroup.interactable = true;
                        fullBackgroundVideoCanvasGroup.blocksRaycasts = true;
                    });

                // update flags
                if (toCam == "one")
                {
                    isPlayingCameraOne = true;
                    isPlayingCameraTwo = false;

                    VideoToggle(fenceIntrusionVideoPlayerOne, true);
                    VideoToggle(fenceIntrusionVideoRawImageTwo, false);

                }
                else
                {
                    isPlayingCameraTwo = true;
                    isPlayingCameraOne = false;

                    VideoToggle(fenceIntrusionVideoPlayerOne, false);
                    VideoToggle(fenceIntrusionVideoRawImageTwo, true);

                }
            });
    }

    private void PlayCamera(RenderTexture texture, string cam)
    {
        fullBackgroundVideoPlayer.texture = texture;

        if (blackBGOverlay.alpha < 1f)
            blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 1f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine).id;
        LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 1f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullBackgroundVideoCanvasGroup.interactable = true;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = true;
            });

        if (cam == "one") isPlayingCameraOne = true;
        else isPlayingCameraTwo = true;

        if (cam == "one")
        {
            VideoToggle(fenceIntrusionVideoPlayerOne, true);
        }
        else
        {
            VideoToggle(fenceIntrusionVideoRawImageTwo, true);
        }
    }

    private void StopCamera(string cam)
    {
        LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullBackgroundVideoPlayer.texture = null;

                fullBackgroundVideoCanvasGroup.interactable = false;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = false;

                if (cam == "one") isPlayingCameraOne = false;
                else isPlayingCameraTwo = false;

                if (cam == "one")
                {
                    VideoToggle(fenceIntrusionVideoPlayerOne, false);
                }
                else
                {
                    VideoToggle(fenceIntrusionVideoRawImageTwo, false);
                }
            });
        SoundManager.Instance.StopLoopedSFX();
        blackBGOverlayId = LeanTween.alphaCanvas(blackBGOverlay, 0f, blackBGOverlayeDuration)
            .setEase(LeanTweenType.easeInOutSine).id;
    }
}
