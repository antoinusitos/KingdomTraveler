using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;

    private AudioSource audioSource = null;
    public AudioSource actionAudioSource = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.time = Random.Range(0, audioSource.clip.length);
    }

    public void PlayActionSound()
    {
        actionAudioSource.Play();
    }
}
