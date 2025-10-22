using UnityEngine;
using UnityEngine.Video;

public class FullScreenVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private Animator animator;
    [SerializeField] private VideoClip[] videoClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        animator = GetComponent<Animator>();
    }

    public void PlayVideoFeed(int CCTVFeedIndex)
    {
        animator.SetTrigger("Open");
        switch ((CCTVFeed)CCTVFeedIndex)
        {
            case CCTVFeed.field:
                videoPlayer.clip = videoClips[0];
                break;
            case CCTVFeed.pool:
                videoPlayer.clip = videoClips[1];
                break;
            case CCTVFeed.blockB:
                videoPlayer.clip = videoClips[2];
                break;
            case CCTVFeed.blockE:
                videoPlayer.clip = videoClips[3];
                break;
            default:
                break;
        }
        videoPlayer.Play();
    }

    public void CloseVideoFeed()
    {
        videoPlayer.Pause();
        animator.SetTrigger("Close");
    }

    
}
