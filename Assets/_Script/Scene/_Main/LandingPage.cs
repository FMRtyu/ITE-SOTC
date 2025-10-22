using UnityEngine;
using UnityEngine.Video;

public class LandingPage : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private VideoClip transitionVideoClip;
    [SerializeField] private VideoClip homeTransitionVideoClip;
    
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject landingPageTexts;
    [SerializeField] private CanvasController canvasController;
    [SerializeField] private Animator digitAnimator;

    [SerializeField] private CampusEventPanel getCampusEventPanel;
    [SerializeField] private CampusEventPanel getCampusEventPanel2;

    [SerializeField] private HomePage homePage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeanTween.alphaCanvas(fadeCanvasGroup, 0f, 2f).setOnComplete(() =>
        {
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        });
    }

    public void StartHome()
    {
        LeanTween.scale(landingPageTexts, new Vector3(0, 1, 1), 0.5f).setOnComplete(() =>
        {
            videoPlayer.clip = transitionVideoClip;
            videoPlayer.Play();

            videoPlayer.loopPointReached += OnTransitionVideoEnd;

        });
    }

    private void OnTransitionVideoEnd(VideoPlayer vp)
    {
        // Unsubscribe so it doesn't trigger multiple times
        vp.loopPointReached -= OnTransitionVideoEnd;
        videoPlayer.clip = homeTransitionVideoClip;

        //campus event
        LeanTween.alphaCanvas(getCampusEventPanel.gameObject.GetComponent<CanvasGroup>(), 1f, 0.5f).setDelay(3f).setOnComplete(() =>
        {
            //different panel refresh
            getCampusEventPanel.RefreshEvents();
        });


        //play video
        videoPlayer.Play();
        digitAnimator.SetTrigger("Open");
        videoPlayer.loopPointReached += OnHomeTransitionVideoEnd;

    }

    private void OnHomeTransitionVideoEnd(VideoPlayer vp)
    {
        // Unsubscribe so it doesn't trigger multiple times
        vp.loopPointReached -= OnHomeTransitionVideoEnd;

        //showHome + static image
        videoPlayer.Stop();

        canvasController.HomeScene();
        getCampusEventPanel2.RefreshEvents();
    }
}
