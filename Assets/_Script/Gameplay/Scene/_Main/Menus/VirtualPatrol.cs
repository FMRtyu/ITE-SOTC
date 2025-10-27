using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VirtualPatrol : _MenuState
{
    [Header("Video elements")]
    [SerializeField] private VideoPlayer fullscreenVideoPlayer;
    [SerializeField] private RawImage fullBackgroundVideoPlayer;

    [SerializeField] private Button fullShrinkButtonOne;
    [SerializeField] private Button fullShrinkButtonTwo;

    [Header("video clips")]
    [SerializeField] private VideoClip[] fullscreenVideoClip;
    [SerializeField] private RenderTexture fenceCameraOneTexture;
    [SerializeField] private RenderTexture fenceCameraTwoTexture;

    [Header("Button textures")]
    [SerializeField] private Sprite fullTexture;
    [SerializeField] private Sprite shrinkTexture;

    //video player settings
    private CanvasGroup fullscreenVideoCanvasGroup;
    private CanvasGroup fullBackgroundVideoCanvasGroup;

    //variables
    private bool isPlayingCameraOne = false;
    private bool isPlayingCameraTwo = false;


    void Start()
    {
        initVirtualPatrol();
    }

    private void OnDisable()
    {
        if (isPlayingCameraOne)
            StopCamera(fullShrinkButtonOne, "one");

        if (isPlayingCameraTwo)
            StopCamera(fullShrinkButtonTwo, "two");
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

        fullShrinkButtonOne.onClick.AddListener(() => ToggleFenceCameraViewOne());
        fullShrinkButtonTwo.onClick.AddListener(() => ToggleFenceCameraViewTwo());

    }
    public override void InitState(DashboardController dashboardController)
    {
        base.InitState(dashboardController);

        state = MenuState.VirtualPatrol;
    }

    public void VideoToggle(VideoPlayer vp)
    {
        if (vp.isPlaying)
        {
            vp.Pause();
        }
        else
        {
            vp.Play();
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
                SwitchCamera(fullShrinkButtonTwo, fenceCameraOneTexture, fullShrinkButtonOne, "one");
            else
                PlayCamera(fullShrinkButtonOne, fenceCameraOneTexture, "one");
        }
        else
        {
            StopCamera(fullShrinkButtonOne, "one");
        }
    }

    public void ToggleFenceCameraViewTwo()
    {
        if (!isPlayingCameraTwo)
        {
            if (isPlayingCameraOne)
                SwitchCamera(fullShrinkButtonOne, fenceCameraTwoTexture, fullShrinkButtonTwo, "two");
            else
                PlayCamera(fullShrinkButtonTwo, fenceCameraTwoTexture, "two");
        }
        else
        {
            StopCamera(fullShrinkButtonTwo, "two");
        }
    }

    // === Smooth transition between two cameras ===
    private void SwitchCamera(Button fromButton, RenderTexture toTexture, Button toButton, string toCam)
    {
        LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 0f, 0.3f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fromButton.image.sprite = fullTexture;
                fullBackgroundVideoPlayer.texture = toTexture;
                toButton.image.sprite = shrinkTexture;

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
                }
                else
                {
                    isPlayingCameraTwo = true;
                    isPlayingCameraOne = false;
                }
            });
    }

    private void PlayCamera(Button targetButton, RenderTexture texture, string cam)
    {
        targetButton.image.sprite = shrinkTexture;
        fullBackgroundVideoPlayer.texture = texture;

        LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 1f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                fullBackgroundVideoCanvasGroup.interactable = true;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = true;
            });

        if (cam == "one") isPlayingCameraOne = true;
        else isPlayingCameraTwo = true;
    }

    private void StopCamera(Button targetButton, string cam)
    {
        LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                targetButton.image.sprite = fullTexture;
                fullBackgroundVideoPlayer.texture = null;

                fullBackgroundVideoCanvasGroup.interactable = false;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = false;

                if (cam == "one") isPlayingCameraOne = false;
                else isPlayingCameraTwo = false;
            });
    }
}
