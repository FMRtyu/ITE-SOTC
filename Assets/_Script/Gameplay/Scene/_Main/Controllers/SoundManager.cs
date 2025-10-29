using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviourSingletonPersistent<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClipData> audioClips = new List<AudioClipData>();

    public void PlaySFX(string clipName)
    {
        var clipData = audioClips.Find(clip => clip.clipName == clipName);
        if (clipData != null && clipData.audioClip != null)
        {
            if (clipData.loop)
            {
                if (loopSource.isPlaying && loopSource.clip == clipData.audioClip)
                    return; // Already playing this looped clip

                loopSource.clip = clipData.audioClip;
                loopSource.volume = clipData.clipVolume;
                loopSource.loop = true;
                loopSource.Play();
            }
            else
            {
                sfxSource.PlayOneShot(clipData.audioClip, clipData.clipVolume);
            }
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Audio clip '{clipName}' not found!");
        }

    }

    public void StopLoopedSFX()
    {
        if (loopSource.isPlaying)
        {
            loopSource.Stop();
            loopSource.clip = null;
        }
    }
}
