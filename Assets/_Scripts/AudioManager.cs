using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Rain")]
    public AudioSource rainSource;
    public AudioLowPassFilter rainLowPass;
    [Range(0f, 1f)] public float rainVolume = 0.6f;
    [Range(10f, 22000f)] public float menuCutoff = 22000f;
    [Range(10f, 22000f)] public float gameCutoff = 3000f;
    [Range(10f, 22000f)] public float convictionCutoff = 500f;

    [Header("Thunder")]
    public AudioSource thunderSource;
    public AudioClip thunderClip;

    [Header("UI")]
    public AudioSource uiSource;
    public AudioClip clickClip;
    public AudioClip hoverClip;
    public AudioClip positiveClip;
    public AudioClip negativeClip;

    [Header("NPC Voice One Shots")]
    public AudioSource npcVoiceSource;
    public AudioClip voiceLow;
    public AudioClip voiceNormal;
    public AudioClip voiceHigh;
    [Range(0f, 0.1f)]
    public float voicePitchVariation = 0.03f; // = +-3%

    Coroutine rainFadeRoutine;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartRainSFX(float volume, float cutoff)
    {
        if (rainSource == null || rainLowPass == null) return;

        if (!rainSource.isPlaying)
            rainSource.Play();

        if (rainFadeRoutine != null)
            StopCoroutine(rainFadeRoutine);

        rainFadeRoutine = StartCoroutine(FadeRainSFX(volume, cutoff, 0.8f));
    }

    IEnumerator FadeRainSFX(float targetVolume, float targetCutoff, float duration)
    {
        float startVol = rainSource.volume;
        float startCut = rainLowPass.cutoffFrequency;

        float t = 0f;
        if (duration <= 0f) duration = 0.0001f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            if (k > 1f) k = 1f;

            rainSource.volume = Mathf.Lerp(startVol, targetVolume, k);
            rainLowPass.cutoffFrequency = targetCutoff;

            yield return null;
        }

        rainSource.volume = targetVolume;
        rainLowPass.cutoffFrequency = targetCutoff;
        rainFadeRoutine = null;
    }

    public void PlayThunder()
    {
        if (thunderSource == null || thunderClip == null) return;
        thunderSource.PlayOneShot(thunderClip);
    }

    public void PlayUIClick() { PlayUI(clickClip, 0.7f); }
    public void PlayUIHover() { PlayUI(hoverClip, 0.4f); }
    public void PlayUIPositive() { PlayUI(positiveClip, 0.8f); }
    public void PlayUINegative() { PlayUI(negativeClip, 0.8f); }

    void PlayUI(AudioClip clip, float vol)
    {
        if (uiSource == null || clip == null) return;
        uiSource.PlayOneShot(clip, vol);
    }
    float nextVoiceAllowedTime = 0f;
    public float voiceCooldown = 0.15f;

    public void PlayNpcVoiceOneShot(string voice)
    {
        if (npcVoiceSource == null) return;
        if (Time.time < nextVoiceAllowedTime) return;

        nextVoiceAllowedTime = Time.time + voiceCooldown;

        string v = voice != null ? voice.Trim().ToLowerInvariant() : "normal";

        AudioClip clip = voiceNormal;

        if (v == "low") clip = voiceLow;
        else if (v == "high") clip = voiceHigh;
        else clip = voiceNormal;

        if (clip == null) return;

        // Random pitch: 1.0 +- variation
        float pitchOffset = Random.Range(-voicePitchVariation, voicePitchVariation);
        npcVoiceSource.pitch = 1f + pitchOffset;

        npcVoiceSource.PlayOneShot(clip, 1f);
    }


}
