using UnityEngine;

public class ResultAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioClip resultSound; // テッテレーの音
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = resultSound;
        audioSource.volume = 0.8f;
        audioSource.Play(); // シーン遷移直後に再生
    }
}
