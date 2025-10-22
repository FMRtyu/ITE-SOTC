using UnityEngine;
using UnityEngine.Video;

public class SimpleVideoPlayer : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool isPlaying = true;

    public void TogglePlayPause()
    {
        if (videoPlayer == null)
            return;

        if (isPlaying)
        {
            videoPlayer.Pause();
            isPlaying = false;
        }
        else
        {
            videoPlayer.Play();
            isPlaying = true;
        }
    }
}
