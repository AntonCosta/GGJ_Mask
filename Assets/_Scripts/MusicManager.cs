using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicState
{
    None,
    MainMenu,
    OutsideShot,
    InsideShot,
    TutorialShot,
    GameShot,
    ConvictionPhase,
    MaskFile,
    Win,
    Lose
}

public class MusicManager : MonoBehaviour
{
    [Serializable]
    public class Stem
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float baseVolume = 1f;

        [HideInInspector] public AudioSource source;
        [HideInInspector] public Coroutine fadeRoutine;
    }

    [Header("Stems")]
    public Stem bass_arp;
    public Stem bass;
    public Stem bells;
    public Stem chords;
    public Stem high;
    public Stem low;
    public Stem pedal;
    public Stem percussion;

    [Header("Timing")]
    [Tooltip("If you know your loop length, set it. Used for randomization ticks.")]
    public float loopLengthSeconds = 23.727f;

    [Header("Conviction percussion lowpass")]
    public float convictionPercussionLowpassHz = 3000f;

    [Header("Randomization")]
    [Range(0, 8)] public int randomActiveCount = 4;
    [Tooltip("Which stems are allowed to be randomized in GameShot and ConvictionPhase.")]
    public bool randomizeBassArp = true;
    public bool randomizeBass = true;
    public bool randomizeBells = true;
    public bool randomizeChords = true;
    public bool randomizeHigh = true;
    public bool randomizeLow = true;
    public bool randomizePedal = true;
    public bool randomizePercussion = false;

    [Header("Global fade defaults")]
    public float fadeOutFast = 0.1f;
    public float fadeInInside = 1.454f;
    public float fadeTutorial = 0.6f;
    public float fadeGame = 0.8f;
    public float fadeWin = 1.2f;
    public float fadeLose = 0.4f;

    [Header("Behavior")]
    public bool playOnAwake = true;
    public float scheduledStartDelay = 0.05f;

    [Header("Main Menu")]
    public bool playPedalInMenu = false;
    
    [Header("Mask File Transition")]
    public float maskFileFadeSeconds = 0.35f;
    public float maskFileLowpassFadeSeconds = 0.35f;
    
    [Header("Win End One Shots")]
    public AudioClip end_bass;
    public AudioClip end_bells;
    public AudioClip end_chords;
    public AudioClip end_high;
    public AudioClip end_low;
    public AudioClip end_percussion;
    public AudioClip end_cymbal_crash;

    [Range(0f, 1f)] public float endVolume = 1f;

    [Header("Wrong Guess Distortion")]
    [Range(0f, 1f)] public float wrongDist1 = 0.15f;
    [Range(0f, 1f)] public float wrongDist2 = 0.35f;
    public float wrongDistFade = 0.15f;

    int wrongGuessCount = 0;
    List<AudioDistortionFilter> distFilters = new List<AudioDistortionFilter>();
    Coroutine distRoutine;


    
    MusicState currentState = MusicState.None;

    Coroutine loopRoutine;
    System.Random rng = new System.Random();

    AudioLowPassFilter percussionLowPass;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        EnsureSource(ref bass_arp);
        EnsureSource(ref bass);
        EnsureSource(ref bells);
        EnsureSource(ref chords);
        EnsureSource(ref high);
        EnsureSource(ref low);
        EnsureSource(ref pedal);
        EnsureSource(ref percussion);
        
        CacheDistortionFilters();


        // Keep a lowpass filter on percussion so we can toggle it in conviction.
        if (percussion.source != null)
        {
            percussionLowPass = percussion.source.GetComponent<AudioLowPassFilter>();
            if (percussionLowPass == null) percussionLowPass = percussion.source.gameObject.AddComponent<AudioLowPassFilter>();
            percussionLowPass.enabled = false;
        }

        if (playOnAwake)
        {
            PlayAllScheduled(scheduledStartDelay);
            // Default state (your spec: main menu plays all)
            SetState(MusicState.MainMenu, 0f);
        }
    }

    void EnsureSource(ref Stem stem)
    {
        if (stem == null) stem = new Stem();
        if (stem.source != null) return;

        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = true;
        s.spatialBlend = 0f;
        s.clip = stem.clip;
        s.volume = 0f;

        stem.source = s;
        stem.fadeRoutine = null;
    }

    void PlayAllScheduled(float delaySeconds)
    {
        double dspStart = AudioSettings.dspTime + Math.Max(0.0, delaySeconds);

        ScheduleStem(bass_arp, dspStart);
        ScheduleStem(bass, dspStart);
        ScheduleStem(bells, dspStart);
        ScheduleStem(chords, dspStart);
        ScheduleStem(high, dspStart);
        ScheduleStem(low, dspStart);
        ScheduleStem(pedal, dspStart);
        ScheduleStem(percussion, dspStart);
    }

    void ScheduleStem(Stem stem, double dspStart)
    {
        if (stem == null || stem.source == null || stem.clip == null) return;

        stem.source.Stop();
        stem.source.clip = stem.clip;
        stem.source.volume = 0f;
        stem.source.PlayScheduled(dspStart);
    }

    public void SetState(MusicState state)
    {
        SetState(state, -1f);
    }

    public void SetState(MusicState state, float overrideFadeSeconds)
    {
        if (state == MusicState.MainMenu)
        {
            wrongGuessCount = 0;
            FadeDistortion(0f, 0.2f);
        }

        currentState = state;


        if (state == MusicState.MainMenu)
{
    wrongGuessCount = 0;
    FadeDistortion(0f, 0.2f);
}

        // Stop any loop randomization routine and restart if needed.
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }

        // Reset percussion filter
        SetPercussionLowpass(false, 22000f);

        if (state == MusicState.MainMenu)
        {
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeGame;

            FadeTo(bass_arp, bass_arp.baseVolume, d);
            FadeTo(bass, bass.baseVolume, d);
            FadeTo(bells, bells.baseVolume, d);
            FadeTo(chords, chords.baseVolume, d);
            FadeTo(high, high.baseVolume, d);
            FadeTo(low, low.baseVolume, d);

            if (playPedalInMenu)
                FadeTo(pedal, pedal.baseVolume, d);
            else
                FadeTo(pedal, 0f, d);   // mute pedal in menu

            FadeTo(percussion, percussion.baseVolume, d);

            return;
        }

        if (state == MusicState.OutsideShot)
        {
            // Only bells, almost instant fade
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeOutFast;
            MuteAll(d);
            FadeTo(bells, bells.baseVolume, d);
            return;
        }

        if (state == MusicState.InsideShot)
        {
            // bells stays, fade in bass + low over 1.454s
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeInInside;
            MuteAll(d);
            FadeTo(bells, bells.baseVolume, d);
            FadeTo(bass, bass.baseVolume, d);
            FadeTo(low, low.baseVolume, d);
            return;
        }

        if (state == MusicState.TutorialShot)
        {
            // Like inside + percussion
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeTutorial;
            MuteAll(d);
            FadeTo(bells, bells.baseVolume, d);
            FadeTo(bass, bass.baseVolume, d);
            FadeTo(low, low.baseVolume, d);
            FadeTo(percussion, percussion.baseVolume, d);
            return;
        }

        if (state == MusicState.GameShot)
        {
            // All layers except percussion available, but only 4 randomly active per loop.
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeGame;
            StartRandomComboMode(includePercussionAlways: false, enablePercLowpass: false, fadeSeconds: d);
            return;
        }

        if (state == MusicState.ConvictionPhase)
        {
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeGame;
            // Smoothly remove lowpass when leaving file view
            FadePercussionLowpass(false, 22000f,
                overrideFadeSeconds >= 0f ? overrideFadeSeconds : maskFileLowpassFadeSeconds);

            StartRandomComboMode(true, false, d);
            return;
        }

        if (state == MusicState.MaskFile)
        {
            float fd = overrideFadeSeconds >= 0f ? overrideFadeSeconds : maskFileFadeSeconds;

            // Smoothly enable lowpass as we enter the file
            FadePercussionLowpass(true, convictionPercussionLowpassHz,
                overrideFadeSeconds >= 0f ? overrideFadeSeconds : maskFileLowpassFadeSeconds);

            MuteAll(fd);
            FadeTo(chords, chords.baseVolume, fd);
            FadeTo(percussion, percussion.baseVolume, fd);
            return;
        }


        if (state == MusicState.Win)
        {
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeWin;
            FadeAllToBase(d);
            return;
        }

        if (state == MusicState.Lose)
        {
            float d = overrideFadeSeconds >= 0f ? overrideFadeSeconds : fadeLose;
            MuteAll(d);
            return;
        }
    }

    // Call this when player presses guilty and is wrong.
    // Behavior: take away layers and add high + pedal + only 1 random layer.
    public void ApplyWrongGuiltyPenalty()
    {
        if (currentState != MusicState.ConvictionPhase) return;

        wrongGuessCount++;
        
        if (wrongGuessCount == 1) FadeDistortion(wrongDist1, wrongDistFade);
        else FadeDistortion(wrongDist2, wrongDistFade);
        // Stop normal random loop, switch to penalty mode.
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }

        SetPercussionLowpass(false, 22000f);

        // Base: mute all, then force high + pedal + 1 random (and optionally percussion? spec says add high pedal and only 1 random layer instead)
        float d = fadeGame;
        MuteAll(d);
        FadeTo(high, high.baseVolume, d);
        FadeTo(pedal, pedal.baseVolume, d);

        Stem one = PickOneRandomAllowed();
        if (one != null) FadeTo(one, one.baseVolume, d);
    }

    // Call when player is correct and wins (from conviction).
    public void ResolveWin()
    {
        wrongGuessCount = 0;
        FadeDistortion(0f, 0.2f);

        SetState(MusicState.Win);
    }

    // Call when player is wrong again and loses.
    public void ResolveLose()
    {
        wrongGuessCount = 0;
        FadeDistortion(0f, 0.2f);

        SetState(MusicState.Lose);
    }

    void StartRandomComboMode(bool includePercussionAlways, bool enablePercLowpass, float fadeSeconds)
    {
        // Start with a first pick instantly then run loop timer.
        ApplyRandomCombo(includePercussionAlways, enablePercLowpass, fadeSeconds);

        loopRoutine = StartCoroutine(RandomComboLoop(includePercussionAlways, enablePercLowpass, fadeSeconds));
    }

    IEnumerator RandomComboLoop(bool includePercussionAlways, bool enablePercLowpass, float fadeSeconds)
    {
        while (true)
        {
            float wait = loopLengthSeconds > 0.05f ? loopLengthSeconds : 23.727f;
            yield return new WaitForSeconds(wait);

            ApplyRandomCombo(includePercussionAlways, enablePercLowpass, fadeSeconds);
        }
    }

    void ApplyRandomCombo(bool includePercussionAlways, bool enablePercLowpass, float fadeSeconds)
    {
        // Determine candidate pool
        List<Stem> candidates = GetRandomizableStems();

        // If you want to exclude percussion in game, keep it out via randomizePercussion flag.
        int count = Mathf.Clamp(randomActiveCount, 0, candidates.Count);

        List<Stem> picked = PickN(candidates, count);

        // Base: mute all (but keep bells? spec says game has all layers without percussion but only 4 playing. So "all" means candidates set)
        // We'll mute everything first, then bring up picked. This can cause "holes" if fade is long; jam acceptable.
        MuteAll(fadeSeconds);

        // Always include percussion in conviction mode
        if (includePercussionAlways)
        {
            FadeTo(percussion, percussion.baseVolume, fadeSeconds);
            if (enablePercLowpass) SetPercussionLowpass(true, convictionPercussionLowpassHz);
        }

        // Bring up picked stems
        for (int i = 0; i < picked.Count; i++)
            FadeTo(picked[i], picked[i].baseVolume, fadeSeconds);

        // In game/conviction, you said "have chords fade in too" for game shot, but also random logic.
        // If you want chords always-on in game, set randomizeChords=false and manually FadeTo(chords,...)
        // For now: chords participates in random pool if enabled.
    }

    List<Stem> GetRandomizableStems()
    {
        List<Stem> list = new List<Stem>();

        if (randomizeBassArp) list.Add(bass_arp);
        if (randomizeBass) list.Add(bass);
        if (randomizeBells) list.Add(bells);
        if (randomizeChords) list.Add(chords);
        if (randomizeHigh) list.Add(high);
        if (randomizeLow) list.Add(low);
        if (randomizePedal) list.Add(pedal);
        if (randomizePercussion) list.Add(percussion);

        // Remove null or missing clips
        list.RemoveAll(s => s == null || s.source == null || s.clip == null);
        return list;
    }

    Stem PickOneRandomAllowed()
    {
        List<Stem> candidates = GetRandomizableStems();
        // In penalty mode you asked: add high pedal and only 1 random layer instead.
        // Exclude high/pedal to avoid doubling.
        candidates.RemoveAll(s => s == high || s == pedal);
        if (candidates.Count == 0) return null;

        int idx = rng.Next(0, candidates.Count);
        return candidates[idx];
    }

    List<Stem> PickN(List<Stem> candidates, int n)
    {
        List<Stem> pool = new List<Stem>(candidates);
        List<Stem> result = new List<Stem>();

        n = Mathf.Clamp(n, 0, pool.Count);

        for (int i = 0; i < n; i++)
        {
            int idx = rng.Next(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        return result;
    }

    void FadeAllToBase(float duration)
    {
        FadeTo(bass_arp, bass_arp.baseVolume, duration);
        FadeTo(bass, bass.baseVolume, duration);
        FadeTo(bells, bells.baseVolume, duration);
        FadeTo(chords, chords.baseVolume, duration);
        FadeTo(high, high.baseVolume, duration);
        FadeTo(low, low.baseVolume, duration);
        FadeTo(pedal, pedal.baseVolume, duration);
        FadeTo(percussion, percussion.baseVolume, duration);
    }

    void MuteAll(float duration)
    {
        FadeTo(bass_arp, 0f, duration);
        FadeTo(bass, 0f, duration);
        FadeTo(bells, 0f, duration);
        FadeTo(chords, 0f, duration);
        FadeTo(high, 0f, duration);
        FadeTo(low, 0f, duration);
        FadeTo(pedal, 0f, duration);
        FadeTo(percussion, 0f, duration);
    }

    void FadeTo(Stem stem, float target, float duration)
    {
        if (stem == null || stem.source == null) return;

        target = Mathf.Clamp01(target);

        if (stem.fadeRoutine != null)
        {
            StopCoroutine(stem.fadeRoutine);
            stem.fadeRoutine = null;
        }

        if (duration <= 0f)
        {
            stem.source.volume = target;
            return;
        }

        stem.fadeRoutine = StartCoroutine(FadeRoutine(stem, target, duration));
    }

    IEnumerator FadeRoutine(Stem stem, float target, float duration)
    {
        float start = stem.source.volume;
        float t = 0f;

        if (duration <= 0f) duration = 0.0001f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            if (k > 1f) k = 1f;

            stem.source.volume = Mathf.Lerp(start, target, k);
            yield return null;
        }

        stem.source.volume = target;
        stem.fadeRoutine = null;
    }

    void SetPercussionLowpass(bool enabled, float cutoffHz)
    {
        if (percussionLowPass == null) return;
        percussionLowPass.enabled = enabled;
        percussionLowPass.cutoffFrequency = Mathf.Clamp(cutoffHz, 10f, 22000f);
    }

    // Optional convenience calls if you prefer explicit names from GameManager
    public void OnAppQuitFadeOut()
    {
        MuteAll(0.2f);
    }
    
    Coroutine percLowpassRoutine;

    void FadePercussionLowpass(bool enabled, float targetCutoffHz, float durationSeconds)
    {
        if (percussionLowPass == null) return;

        if (percLowpassRoutine != null)
        {
            StopCoroutine(percLowpassRoutine);
            percLowpassRoutine = null;
        }

        if (durationSeconds <= 0f)
        {
            percussionLowPass.enabled = enabled;
            percussionLowPass.cutoffFrequency = Mathf.Clamp(targetCutoffHz, 10f, 22000f);
            return;
        }

        percLowpassRoutine = StartCoroutine(PercLowpassRoutine(enabled, targetCutoffHz, durationSeconds));
    }

    IEnumerator PercLowpassRoutine(bool enabled, float targetCutoffHz, float durationSeconds)
    {
        // If enabling, turn on first so it actually filters during the fade
        if (enabled) percussionLowPass.enabled = true;

        float start = percussionLowPass.cutoffFrequency;
        float end = Mathf.Clamp(targetCutoffHz, 10f, 22000f);

        float t = 0f;
        if (durationSeconds <= 0f) durationSeconds = 0.0001f;

        while (t < durationSeconds)
        {
            t += Time.deltaTime;
            float k = t / durationSeconds;
            if (k > 1f) k = 1f;

            percussionLowPass.cutoffFrequency = Mathf.Lerp(start, end, k);
            yield return null;
        }

        percussionLowPass.cutoffFrequency = end;

        // If disabling, you can disable after restoring a wide cutoff
        if (!enabled)
            percussionLowPass.enabled = false;

        percLowpassRoutine = null;
    }
    
    public void PlayWinEndInstant(float fadeOutGameplaySeconds)
    {
        // Stop combo loop
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }

        currentState = MusicState.Win;

        // Optional: fade gameplay down quickly, then hard stop.
        if (fadeOutGameplaySeconds > 0f)
        {
            MuteAll(fadeOutGameplaySeconds);
            StartCoroutine(StopGameplayAfterSeconds(fadeOutGameplaySeconds));
        }
        else
        {
            StopGameplayNow();
        }

        // Make sure end layers are not looping
        PlayEndOneShot(end_bass, endVolume);
        PlayEndOneShot(end_bells, endVolume);
        PlayEndOneShot(end_chords, endVolume);
        PlayEndOneShot(end_high, endVolume);
        PlayEndOneShot(end_low, endVolume);
        PlayEndOneShot(end_percussion, endVolume);

        // Cymbal crash on top
        PlayEndOneShot(end_cymbal_crash, endVolume);
    }

    void PlayEndOneShot(AudioClip clip, float vol)
    {
        if (clip == null) return;

        // One-shot on this manager (2D).
        // Using PlayOneShot avoids needing extra sources.
        AudioSource s = GetOrCreateEndOneShotSource();
        s.PlayOneShot(clip, Mathf.Clamp01(vol));
    }

    AudioSource endOneShotSource;

    AudioSource GetOrCreateEndOneShotSource()
    {
        if (endOneShotSource != null) return endOneShotSource;

        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = false;
        s.spatialBlend = 0f;
        s.volume = 1f;

        endOneShotSource = s;
        return endOneShotSource;
    }

    IEnumerator StopGameplayAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopGameplayNow();
    }

    void StopGameplayNow()
    {
        StopStem(bass_arp);
        StopStem(bass);
        StopStem(bells);
        StopStem(chords);
        StopStem(high);
        StopStem(low);
        StopStem(pedal);
        StopStem(percussion);
    }

    void StopStem(Stem stem)
    {
        if (stem == null || stem.source == null) return;
        stem.source.Stop();
        stem.source.volume = 0f;
    }

void CacheDistortionFilters()
{
    distFilters.Clear();
    AddDist(bass_arp);
    AddDist(bass);
    AddDist(bells);
    AddDist(chords);
    AddDist(high);
    AddDist(low);
    AddDist(pedal);
    AddDist(percussion);

    SetDistortionImmediate(0f);
}

void AddDist(Stem stem)
{
    if (stem == null || stem.source == null) return;

    AudioDistortionFilter f = stem.source.GetComponent<AudioDistortionFilter>();
    if (f == null) f = stem.source.gameObject.AddComponent<AudioDistortionFilter>();
    f.enabled = false;
    f.distortionLevel = 0f;
    distFilters.Add(f);
}

void SetDistortionImmediate(float level)
{
    level = Mathf.Clamp01(level);
    for (int i = 0; i < distFilters.Count; i++)
    {
        var f = distFilters[i];
        if (f == null) continue;
        f.enabled = level > 0f;
        f.distortionLevel = level;
    }
}

void FadeDistortion(float target, float seconds)
{
    if (distRoutine != null) StopCoroutine(distRoutine);
    distRoutine = StartCoroutine(DistRoutine(target, seconds));
}

IEnumerator DistRoutine(float target, float seconds)
{
    target = Mathf.Clamp01(target);

    float start = 0f;
    if (distFilters.Count > 0 && distFilters[0] != null)
        start = distFilters[0].distortionLevel;

    for (int i = 0; i < distFilters.Count; i++)
    {
        var f = distFilters[i];
        if (f == null) continue;
        if (target > 0f) f.enabled = true;
    }

    float t = 0f;
    if (seconds <= 0f) seconds = 0.0001f;

    while (t < seconds)
    {
        t += Time.deltaTime;
        float k = t / seconds;
        if (k > 1f) k = 1f;

        float v = Mathf.Lerp(start, target, k);

        for (int i = 0; i < distFilters.Count; i++)
        {
            var f = distFilters[i];
            if (f == null) continue;
            f.distortionLevel = v;
        }

        yield return null;
    }

    for (int i = 0; i < distFilters.Count; i++)
    {
        var f = distFilters[i];
        if (f == null) continue;
        f.distortionLevel = target;
        if (target <= 0f) f.enabled = false;
    }

    distRoutine = null;
}


}
