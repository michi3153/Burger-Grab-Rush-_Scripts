using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameStartHandler : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> confirmClip = new List<AudioClip>();
    public string sceneName = "DifficultySelectScene";
    private bool isStarted = false;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // ボタンクリック時の処理を登録
        startButton.onClick.AddListener(SelectedGameStart);

        // 最初はボタンを無効化
        startButton.interactable = false;
    }
    private void Update()
    {
        startButton.interactable = !string.IsNullOrWhiteSpace(nameInputField?.text);
    }

    public void SelectedGameStart()
    {
        if (isStarted) return;

        string playerName = nameInputField?.text ?? "";
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "--"; // デフォルト名
        }

        isStarted = true;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        if (confirmClip.Count > 0 && confirmClip[0] != null)
        {
            audioSource.PlayOneShot(confirmClip[0]);
            Debug.Log("[GameStartHandler] 効果音再生開始");
        }
        else
        {
            Debug.LogWarning("[GameStartHandler] ConfirmClip が設定されていません！");
        }
        isStarted = true;
        Invoke(nameof(LoadDifficultScene), 0.5f);
    }

    private void LoadDifficultScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
