using UnityEngine;
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
    [SerializeField] private VideoClip landingVideoClip;
    [SerializeField] private VideoClip transitionVideoClip;
    [SerializeField] private VideoClip homeTransitionVideoClip;

    //variables
    private int fadeTweenId = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initlandingpage();
    }

    private void initlandingpage()
    {
        //play background video
        landingVideoBackground.clip = landingVideoClip;
        landingVideoBackground.Play();

        //play bgm
        SoundManager.Instance.PlaySFX("theEpic");

        // Begin click to start fade loop
        FadeOutButton();
    }
    #region Video Callbacks
    public void OnClickToStart()
    {
        // Stop the button fade loop
        LeanTween.cancel(fadeTweenId);

        SoundManager.Instance.PlaySFX("button_click");

        // Play transition video
        LeanTween.scale(headerTexts, Vector3.zero, 0.5f).setOnComplete(() =>
        {
            landingVideoBackground.clip = transitionVideoClip;
            landingVideoBackground.Play();
            landingVideoBackground.loopPointReached += OnTransitionVideoEnd;
        });
    }

    private void OnTransitionVideoEnd(VideoPlayer vp)
    {
        // Unsubscribe so it doesn't trigger multiple times
        vp.loopPointReached -= OnTransitionVideoEnd;
        landingVideoBackground.clip = homeTransitionVideoClip;

        uiManager.ShowDashboardCampusEvent();

        landingVideoBackground.Play();
        landingVideoBackground.loopPointReached += OnHomeTransitionVideoEnd;

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

}
