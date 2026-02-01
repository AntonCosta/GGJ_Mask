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
    [Header("UI Writing Sound")]
    public AudioClip writingClip;

    [Range(0f, 0.1f)] public float writingPitchVariation = 0.04f; // +-4%
    [Range(0f, 0.2f)] public float writingVolumeVariation = 0.1f; // +-10%

    [Header("UI NPC Cycle Sound")]
    public AudioClip npcCycleClip;

    [Range(0f, 0.1f)] public float npcCyclePitchVariation = 0.03f;
    [Range(0f, 0.2f)] public float npcCycleVolumeVariation = 0.08f;
    
    [Header("NPC Voice One Shots")]
    public AudioSource npcVoiceSource;
    public AudioClip voiceLow;
    public AudioClip voiceNormal;
    public AudioClip voiceHigh;
    [Range(0f, 0.1f)]
    public float voicePitchVariation = 0.03f; // = +-3%

    [Header("NPC Voice Fade")]
    public float npcVoiceFadeOutSeconds = 0.06f;
    public float npcVoiceFadeInSeconds = 0.03f;

    Coroutine npcVoiceRoutine;

    
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

        string v = voice != null ? voice.Trim().ToLowerInvariant() : "normal";

        AudioClip clip = voiceNormal;
        if (v == "low") clip = voiceLow;
        else if (v == "high") clip = voiceHigh;
        else clip = voiceNormal;

        if (clip == null) return;

        if (npcVoiceRoutine != null)
        {
            StopCoroutine(npcVoiceRoutine);
            npcVoiceRoutine = null;
        }

        npcVoiceRoutine = StartCoroutine(PlayNpcVoiceFadeReplace(clip));
    }

    IEnumerator PlayNpcVoiceFadeReplace(AudioClip clip)
    {
        float startVol = npcVoiceSource.volume;

        // Fade out current voice if something is playing
        if (npcVoiceSource.isPlaying && npcVoiceFadeOutSeconds > 0f)
        {
            float t = 0f;
            while (t < npcVoiceFadeOutSeconds)
            {
                t += Time.deltaTime;
                float k = t / npcVoiceFadeOutSeconds;
                if (k > 1f) k = 1f;
                npcVoiceSource.volume = Mathf.Lerp(startVol, 0f, k);
                yield return null;
            }
        }

        npcVoiceSource.Stop();

        // Random pitch per new line (your +-3% logic)
        float pitchOffset = Random.Range(-voicePitchVariation, voicePitchVariation);
        npcVoiceSource.pitch = 1f + pitchOffset;

        npcVoiceSource.clip = clip;
        npcVoiceSource.volume = 0f;
        npcVoiceSource.Play();

        // Fade in new voice
        float t2 = 0f;
        float fadeIn = npcVoiceFadeInSeconds;
        if (fadeIn <= 0f)
        {
            npcVoiceSource.volume = startVol;
            npcVoiceRoutine = null;
            yield break;
        }

        while (t2 < fadeIn)
        {
            t2 += Time.deltaTime;
            float k2 = t2 / fadeIn;
            if (k2 > 1f) k2 = 1f;
            npcVoiceSource.volume = Mathf.Lerp(0f, startVol, k2);
            yield return null;
        }

        npcVoiceSource.volume = startVol;
        npcVoiceRoutine = null;
    }

    public void PlayUIWriting()
    {
        if (uiSource == null || writingClip == null) return;

        // Random pitch
        float pitchOffset = Random.Range(-writingPitchVariation, writingPitchVariation);
        uiSource.pitch = 1f + pitchOffset;

        // Random volume
        float volOffset = Random.Range(-writingVolumeVariation, writingVolumeVariation);
        float finalVol = Mathf.Clamp01(0.6f + volOffset);

        uiSource.PlayOneShot(writingClip, finalVol);
    }
    
    public void PlayNpcCycle()
    {
        if (uiSource == null || npcCycleClip == null) return;

        float pitchOffset = Random.Range(-npcCyclePitchVariation, npcCyclePitchVariation);
        uiSource.pitch = 1f + pitchOffset;

        float volOffset = Random.Range(-npcCycleVolumeVariation, npcCycleVolumeVariation);
        float finalVol = Mathf.Clamp01(0.6f + volOffset);

        uiSource.PlayOneShot(npcCycleClip, finalVol);
    }
    public void StopNpcVoice(float fadeOutSeconds)
    {
        if (npcVoiceSource == null) return;

        if (npcVoiceRoutine != null)
        {
            StopCoroutine(npcVoiceRoutine);
            npcVoiceRoutine = null;
        }

        npcVoiceRoutine = StartCoroutine(FadeOutAndStopNpcVoice(fadeOutSeconds));
    }

    IEnumerator FadeOutAndStopNpcVoice(float fadeOutSeconds)
    {
        if (!npcVoiceSource.isPlaying)
        {
            npcVoiceRoutine = null;
            yield break;
        }

        float startVol = npcVoiceSource.volume;

        if (fadeOutSeconds <= 0f)
        {
            npcVoiceSource.Stop();
            npcVoiceSource.volume = startVol;
            npcVoiceRoutine = null;
            yield break;
        }

        float t = 0f;
        while (t < fadeOutSeconds)
        {
            t += Time.deltaTime;
            float k = t / fadeOutSeconds;
            if (k > 1f) k = 1f;
            npcVoiceSource.volume = Mathf.Lerp(startVol, 0f, k);
            yield return null;
        }

        npcVoiceSource.Stop();
        npcVoiceSource.volume = startVol;
        npcVoiceRoutine = null;
    }





}
