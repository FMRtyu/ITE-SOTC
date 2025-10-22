using UnityEngine;

public class SimpleButtonFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private int fadeTweenId = -1;

    private void Start()
    {
        // If CanvasGroup not set manually, get it automatically
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        StartFadeLoop();
    }

    public void StartFadeLoop()
    {
        // Stop any existing tween first
        LeanTween.cancel(fadeTweenId);

        // Begin looping fade
        FadeOut();
    }

    private void FadeIn()
    {
        fadeTweenId = LeanTween.value(gameObject, 0f, 1f, fadeDuration)
            .setOnUpdate((float val) => canvasGroup.alpha = val)
            .setOnComplete(FadeOut)
            .id;
    }

    private void FadeOut()
    {
        fadeTweenId = LeanTween.value(gameObject, 1f, 0f, fadeDuration)
            .setOnUpdate((float val) => canvasGroup.alpha = val)
            .setOnComplete(FadeIn)
            .id;
    }

    public void StopFade()
    {
        LeanTween.cancel(fadeTweenId);
    }
}
