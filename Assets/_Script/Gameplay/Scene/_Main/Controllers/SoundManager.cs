using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SoundManager : MonoBehaviourSingletonPersistent<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClipData> audioClips = new List<AudioClipData>();

    private LTDescr currentFadeTween; // store current tween so it can be cancelled if needed

    public void PlaySFX(string clipName)
    {
        var clipData = audioClips.Find(clip => clip.clipName == clipName);
        if (clipData != null && clipData.audioClip != null)
        {
            if (clipData.loop)
            {
                if (loopSource.isPlaying && loopSource.clip == clipData.audioClip)
                    return;

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

    public void FadeOutLoopedSFX(float duration = 1f)
    {
        if (loopSource.isPlaying)
        {
            // cancel previous tween if still running
            if (currentFadeTween != null)
                LeanTween.cancel(currentFadeTween.id);

            float startVolume = loopSource.volume;
            currentFadeTween = LeanTween.value(gameObject, startVolume, 0f, duration)
                .setOnUpdate((float v) => loopSource.volume = v)
                .setOnComplete(() =>
                {
                    loopSource.Stop();
                    loopSource.clip = null;
                });
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

    // optional: adjust any clip volume smoothly
    public void SetVolume(float newVolume, string clipName, float duration = 0.5f)
    {
        var clipData = audioClips.Find(c => c.clipName == clipName);
        if (clipData != null)
        {
            clipData.clipVolume = newVolume;
            if (loopSource.clip == clipData.audioClip)
            {
                LeanTween.value(gameObject, loopSource.volume, newVolume, duration)
                    .setOnUpdate((float v) => loopSource.volume = v);
            }
        }
    }

    public float GetVolumeValue(bool loop = false)
    {
        if (loop)
        {
            return loopSource.volume;
        }
        else
        {
            return sfxSource.volume;
        }
    }
}
