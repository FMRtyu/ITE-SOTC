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

    void OnDisable()
    {
        if (isPlayingCameraOne)
        {
            ToggleFenceCameraViewOne();
        }
        if (isPlayingCameraTwo)
        {
            ToggleFenceCameraViewTwo();
        }
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
            //full view
            fullShrinkButtonOne.image.sprite = shrinkTexture;
            fullBackgroundVideoPlayer.texture = fenceCameraOneTexture;

            LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                fullBackgroundVideoCanvasGroup.interactable = true;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = true;
            });

            isPlayingCameraOne = true;
        }
        else
        {
            //shrink view
            LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                fullShrinkButtonOne.image.sprite = fullTexture;
                fullBackgroundVideoPlayer.texture = null;

                fullBackgroundVideoCanvasGroup.interactable = false;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = false;

                isPlayingCameraOne = false;
            });
        }
    }

    public void ToggleFenceCameraViewTwo()
    {
        if (!isPlayingCameraTwo)
        {
            //full view
            fullShrinkButtonTwo.image.sprite = shrinkTexture;
            fullBackgroundVideoPlayer.texture = fenceCameraTwoTexture;

            LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                fullBackgroundVideoCanvasGroup.interactable = true;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = true;
            });

            isPlayingCameraTwo = true;
        }
        else
        {
            //shrink view
            LeanTween.alphaCanvas(fullBackgroundVideoCanvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                fullShrinkButtonTwo.image.sprite = fullTexture;

                fullBackgroundVideoPlayer.texture = null;

                fullBackgroundVideoCanvasGroup.interactable = false;
                fullBackgroundVideoCanvasGroup.blocksRaycasts = false;

                isPlayingCameraTwo = false;
            });
        }
    }
}
