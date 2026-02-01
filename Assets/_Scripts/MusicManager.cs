using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Serializable]
    public class Layer
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float initialVolume = 0f;

        [HideInInspector] public AudioSource source;
        [HideInInspector] public Coroutine fadeRoutine;
    }

    [Header("Looping stems")]
    public List<Layer> layers = new List<Layer>();

    [Header("Auto")]
    public bool playOnStart = true;
    public float startDelaySeconds = 0.05f;

    void Awake()
    {
        // Create one AudioSource per layer
        for (int i = 0; i < layers.Count; i++)
        {
            Layer l = layers[i];
            if (l == null) continue;

            AudioSource s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.loop = true;
            s.spatialBlend = 0f; // 2D
            s.clip = l.clip;
            s.volume = Mathf.Clamp01(l.initialVolume);

            l.source = s;
            l.fadeRoutine = null;
        }
    }

    void Start()
    {
        if (playOnStart)
        {
            PlayAllScheduled(startDelaySeconds);
        }
    }

    // Starts all stems in sync using DSP scheduling
    public void PlayAllScheduled(float delaySeconds)
    {
        double dspStart = AudioSettings.dspTime + Math.Max(0.0, delaySeconds);

        for (int i = 0; i < layers.Count; i++)
        {
            Layer l = layers[i];
            if (l == null || l.source == null || l.clip == null) continue;

            l.source.Stop();
            l.source.clip = l.clip;
            l.source.PlayScheduled(dspStart);
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            Layer l = layers[i];
            if (l == null || l.source == null) continue;

            StopFade(l);
            l.source.Stop();
            l.source.volume = 0f;
        }
    }

    // Fade one layer to target volume (0..1). durationSeconds <= 0 means instant.
    public void FadeLayer(string layerName, float targetVolume01, float durationSeconds)
    {
        Layer l = FindLayer(layerName);
        if (l == null || l.source == null) return;

        float target = Mathf.Clamp01(targetVolume01);

        StopFade(l);

        if (durationSeconds <= 0f)
        {
            l.source.volume = target;
            return;
        }

        l.fadeRoutine = StartCoroutine(FadeRoutine(l, target, durationSeconds));
    }

    // Fade all layers to the same target volume
    public void FadeAll(float targetVolume01, float durationSeconds)
    {
        float t = Mathf.Clamp01(targetVolume01);
        for (int i = 0; i < layers.Count; i++)
        {
            Layer l = layers[i];
            if (l == null) continue;
            FadeLayer(l.name, t, durationSeconds);
        }
    }

    // Optional helper: set initial mix quickly
    public void SetMixInstant(params (string name, float volume)[] mix)
    {
        for (int i = 0; i < mix.Length; i++)
        {
            FadeLayer(mix[i].name, mix[i].volume, 0f);
        }
    }

    IEnumerator FadeRoutine(Layer l, float target, float durationSeconds)
    {
        float start = l.source.volume;
        float time = 0f;

        while (time < durationSeconds)
        {
            time += Time.unscaledDeltaTime;
            float a = time / durationSeconds;
            if (a > 1f) a = 1f;

            l.source.volume = Mathf.Lerp(start, target, a);
            yield return null;
        }

        l.source.volume = target;
        l.fadeRoutine = null;
    }

    void StopFade(Layer l)
    {
        if (l.fadeRoutine != null)
        {
            StopCoroutine(l.fadeRoutine);
            l.fadeRoutine = null;
        }
    }

    Layer FindLayer(string layerName)
    {
        if (string.IsNullOrEmpty(layerName)) return null;

        for (int i = 0; i < layers.Count; i++)
        {
            Layer l = layers[i];
            if (l == null) continue;

            if (string.Equals(l.name, layerName, StringComparison.OrdinalIgnoreCase))
                return l;
        }

        return null;
    }
}
