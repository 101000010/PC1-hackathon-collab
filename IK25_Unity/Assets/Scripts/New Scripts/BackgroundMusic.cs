using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField]
    private AudioClip audioClip1;

    [SerializeField]
    private AudioClip audioClip2;

    [SerializeField]
    private AudioClip audioClip3;

    [SerializeField]
    private AudioClip audioClip4; 

    [SerializeField]
    private AudioClip audioClip5;

    [SerializeField]
    private AudioSource audioSource;

    private float gameTime;
    private int currentClipIndex = 0;
    private AudioClip[] audioClips;
    private float fadeDuration = 2f; // Duration of the fade in seconds
    private float targetVolume = 1f; // Target volume level
    private float reducedVolume1 = 0.6f; // Reduced volume level for audioClip4
    private float reducedVolume2 = 0.4f; // Reduced volume level for audioClip5

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
            return;
        }

        audioSource.volume = targetVolume; // Set the initial volume to the target volume
        audioClips = new AudioClip[] { audioClip1, audioClip2, audioClip3, audioClip4, audioClip5 };
        PlayAudioClip(audioClips[currentClipIndex]);
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime >= 30)
        {
            gameTime = 0; // Reset game time for the next clip
            currentClipIndex = (currentClipIndex + 1) % audioClips.Length; // Move to the next clip and loop back to the first clip
            StartCoroutine(FadeOutAndPlayNextClip(audioClips[currentClipIndex]));
        }
    }

    private void PlayAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.time = 30f; // Start playing from the 30-second mark
        audioSource.Play();

        // Adjust volume for specific clips
        if (clip == audioClip3)
        {
            audioSource.volume = reducedVolume1;
        }
        if (clip == audioClip4)
        {
            audioSource.volume = reducedVolume2;
        }
        else if (clip == audioClip5)
        {
            audioSource.volume = reducedVolume2;
        }
        else
        {
            audioSource.volume = targetVolume;
        }
    }

    private IEnumerator FadeOutAndPlayNextClip(AudioClip nextClip)
    {
        // Fade out
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();

        // Play next clip
        PlayAudioClip(nextClip);

        // Fade in
        float targetVolumeForNextClip = (nextClip == audioClip4) ? reducedVolume1 : (nextClip == audioClip5) ? reducedVolume2 : targetVolume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolumeForNextClip, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolumeForNextClip;
    }
}