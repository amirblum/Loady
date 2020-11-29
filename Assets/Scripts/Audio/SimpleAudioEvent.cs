using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip[] clips;

    public RangedFloat volume = new RangedFloat(0.95f, 1f);

    [MinMaxRange(0, 2)]
    public RangedFloat pitch = new RangedFloat(0.95f, 1.05f);

    public AudioMixerGroup audioGroup;

    public override void Play(Vector3 position = new Vector3())
    {
        if (clips.Length == 0) return;

        var audioPlayer = new GameObject("audioPlayer", typeof(AudioSource)).GetComponent<AudioSource>();
//        audioPlayer.transform.position = position;
        audioPlayer.outputAudioMixerGroup = audioGroup;

        Play(audioPlayer);

        Destroy(audioPlayer.gameObject, audioPlayer.clip.length * audioPlayer.pitch);
    }

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        if (source == null)
        {
            var audioPlayer = new GameObject("audioPlayer", typeof(AudioSource)).GetComponent<AudioSource>();
            audioPlayer.transform.position = new Vector3();
            audioPlayer.outputAudioMixerGroup = audioGroup;

            audioPlayer.clip = clips[Random.Range(0, clips.Length)];
            audioPlayer.volume = Random.Range(volume.minValue, volume.maxValue);
            audioPlayer.pitch = Random.Range(pitch.minValue, pitch.maxValue);
            audioPlayer.Play();

            Destroy(audioPlayer.gameObject, audioPlayer.clip.length * audioPlayer.pitch);
        }

        else
        {
            source.clip = clips[Random.Range(0, clips.Length)];
            source.volume = Random.Range(volume.minValue, volume.maxValue);
            source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
            source.Play();
        }
    }

    public override void PlatUISound()
    {
        Play();
    }
}