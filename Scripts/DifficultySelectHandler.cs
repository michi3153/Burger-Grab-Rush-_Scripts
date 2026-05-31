using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class DifficultySelectHandler : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> audioClip = new List<AudioClip>();
    private bool isStarted = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void SelectDifficulty(int difficultyIndex)
    {
        if (isStarted) return;
        isStarted = true;

        if (audioClip.Count > 0 && audioClip[0] != null)
        {
            audioSource.PlayOneShot(audioClip[0]);
            Debug.Log("[DifficultySelectHandler] 効果音再生開始");
        }
        else
        {
            Debug.LogWarning("[DifficultySelectHandler] AudioClip が設定されていません！");
        }

        GameManager.Instance.SetDifficulty((GameManager.Difficulty)difficultyIndex);

        // 0.5秒後に遷移（効果音の長さに合わせて調整）
        Invoke(nameof(LoadMainScene), 0.5f);
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("mainscene");
    }
}
