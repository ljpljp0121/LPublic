
using UnityEngine;

[System.Serializable]
public class ChronosAudioSource : ChronosMonoBehaviour
{
    public ChronosComponent chronosComponent;
    public AudioSource[] audioSources;

    private float[] cachedPitched;
    protected float cachedTimeScale;

    public float minTimeScale = 0f;
    public float maxTimeScale = 2f;
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;

    protected virtual void Awake()
    {
        OnValidate();
    }

    protected virtual void Update()
    {
        if (Mathf.Approximately(chronosComponent.TimeScale, cachedTimeScale))
            return;

        cachedTimeScale = chronosComponent.TimeScale;
    }

    protected virtual void OnValidate()
    {
        if (audioSources == null || audioSources.Length == 0)
            audioSources = GetComponentsInChildren<AudioSource>();

        if (chronosComponent == null)
            chronosComponent = GetComponent<ChronosComponent>();
    }
}
