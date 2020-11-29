using UnityEngine;

public abstract class AudioEvent : ScriptableObject
{
    public abstract void Play(Vector3 position = new Vector3());
    public abstract void Play(AudioSource source);

    public abstract void PlatUISound();
}
