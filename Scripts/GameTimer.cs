using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Leap;
using static GameManager;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float totalTime = 10f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timeUpText;
    [SerializeField] private Image sandClockImage;
    [SerializeField] private GameObject timeUpPanel;
    [SerializeField] private GameObject ghostHands;
    [SerializeField] private GameObject provider;

    private float currentTime;
    private bool isGameActive = true;
    private bool isFlashing = false;

    public ScoreManager scoreManager;
    public BurgerJudgeSystem judgeSystem;

    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip timeUpSound; // 追加: タイムアップ音声
    AudioSource musicSource;
    AudioSource timeUpAudioSource; // 追加: タイムアップ音声用のAudioSource
    private bool hasStartedMusic = false; // ← これを class 内に追加

    Color redTextColor = new Color(250f / 255f, 57f / 255f, 50f / 255f);

    void Start()
    {
        currentTime = totalTime;
        timeUpPanel.gameObject.SetActive(false);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = gameMusic;
        musicSource.loop = true;
        musicSource.volume = 1.0f; // 好みで調整

        timeUpAudioSource = gameObject.AddComponent<AudioSource>();
        timeUpAudioSource.playOnAwake = false;
        timeUpAudioSource.volume = 1.0f;
    }

    void Update()
    {
        if (!isGameActive) return;

        if (GameManager.Instance.CurrentGameState != GameState.Playing) return;

        if (GameManager.Instance.CurrentGameState == GameState.Playing && !hasStartedMusic)
        {
            musicSource.Play();
            hasStartedMusic = true; // 1回だけ再生
        }

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0);

        UpdateTimerUI(currentTime);

        if (currentTime <= 10f && !isFlashing)
        {
            StartCoroutine(FlashText());
            StartCoroutine(ShakeImage());
            isFlashing = true;
        }

        if (currentTime <= 0f)
        {
            isGameActive = false;
            ShowTimeUpPanel();
        }
    }

    private void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // 点滅処理
    private IEnumerator FlashText()
    {
        while (currentTime > 0f)
        {
            timerText.color = redTextColor;
            yield return new WaitForSeconds(0.5f);
            timerText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // 砂時計画像の震え処理
    private IEnumerator ShakeImage()
    {
        Vector3 originalPos = sandClockImage.rectTransform.localPosition;
        while (currentTime > 0f)
        {
            Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
            sandClockImage.rectTransform.localPosition = originalPos + offset;
            yield return new WaitForSeconds(0.05f);
        }
        sandClockImage.rectTransform.localPosition = originalPos;
    }

    void ShowTimeUpPanel()
    {
        timeUpPanel.SetActive(true);

        // BGM停止
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        if (ghostHands != null)
        {
            ghostHands.SetActive(false);
            provider.SetActive(false);
        }

        // 全処理の停止
        GameManager.Instance.SetGameState(GameManager.GameState.Ended);

        // スコア保存（必要があれば）
        GameManager.Instance.FinalScore = scoreManager.currentScore;
        GameManager.Instance.SuccessCount = judgeSystem.CorrectCount;
        GameManager.Instance.FailCount = judgeSystem.FailCount;

        // 一定時間待ってからリザルト遷移
        StartCoroutine(PlayTimeUpSoundAndGoToResult());
    }
    IEnumerator PlayTimeUpSoundAndGoToResult()
    {
        timeUpAudioSource.PlayOneShot(timeUpSound);
        yield return new WaitForSeconds(timeUpSound.length+1.0f);

        SceneManager.LoadScene("ResultScene");
    }
}
