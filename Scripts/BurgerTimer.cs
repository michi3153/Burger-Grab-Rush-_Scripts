using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static GameManager;

public class BurgerTimer : MonoBehaviour
{
    public Slider timeSlider; // スライダーUI
    public Image fillImage;   // スライダーの塗り部分（Fill Area > Fill）
    public Image backgroundImage; // スライダーの背景部分（Background）
    public Image BoardImage;

    public Image burgerImage;
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    public float totalTime = 25f; // デフォルト提供時間

    private float currentTime;
    private bool isRunning = false;
    private bool isShaking = false;

    public Action<BurgerTimer> OnTimeOut; // 時間切れ時のイベント
    private Coroutine shakeCoroutine;

    private void Start()
    {
        ResetTimer();
        StartTimer();
    }
    public void StartTimer(float customTime = -1f)
    {
        this.enabled = true;

        currentTime = customTime > 0f ? customTime : totalTime;
        isRunning = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        StopTimer();
        currentTime = totalTime;
        UpdateUI();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing) return;

        if (!isRunning)
        {
            Debug.Log("slider減らないよ");
            return;
        }

        currentTime -= Time.deltaTime;
        //Debug.Log("Timer running: " + currentTime);
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            UpdateUI();
            OnTimeOut?.Invoke(this); // 通知
        }
        else
        {
            UpdateUI();
            // 残り3秒以下になったら1度だけ揺らす
            if (currentTime <= 3f && !isShaking)
            {
                StartCoroutine(ShakeUIElements());
            }
        }
    }

    private void UpdateUI()
    {
        if (timeSlider != null)
        {
            timeSlider.value = currentTime / totalTime;
            //Debug.Log($"value:{timeSlider.value}");
        }

        if (fillImage != null)
        {
            float ratio = currentTime / totalTime;

            if (ratio <= 0.2f)
                fillImage.color = redColor;
            else if (ratio <= 0.4f)
                fillImage.color = yellowColor;
            else
                fillImage.color = greenColor;
        }
    }

    private IEnumerator ShakeUIElements()
    {
        isShaking = true;

        Vector3 originalFillPos = fillImage.rectTransform.anchoredPosition;
        Vector3 originalBurgerPos = burgerImage != null ? burgerImage.rectTransform.anchoredPosition : Vector3.zero;
        Vector3 originalBackgroundPos = backgroundImage != null ? backgroundImage.rectTransform.anchoredPosition : Vector3.zero;
        Vector3 originalBoardPos = BoardImage != null ? BoardImage.rectTransform.anchoredPosition : Vector3.zero;

        float shakeDuration = 1f;       // 揺れる時間
        float elapsed = 0f;
        float shakeStrength = 3f;         // 揺れの強さ（小さめに）
        float shakeSpeed = 20f;           // 揺れの速さ（振動数）

        while (elapsed < shakeDuration)
        {
            float offset = Mathf.Sin(elapsed * shakeSpeed) * shakeStrength;

            Vector3 shakeOffset = new Vector3(offset, 0f, 0f); // 横揺れのみ

            fillImage.rectTransform.anchoredPosition = originalFillPos + shakeOffset;

            if (burgerImage != null)
                burgerImage.rectTransform.anchoredPosition = originalBurgerPos + shakeOffset;

            if (backgroundImage != null)
                backgroundImage.rectTransform.anchoredPosition = originalBackgroundPos + shakeOffset;

            if (BoardImage != null)
                BoardImage.rectTransform.anchoredPosition = originalBoardPos + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 元の位置に戻す
        fillImage.rectTransform.anchoredPosition = originalFillPos;

        if (burgerImage != null)
            burgerImage.rectTransform.anchoredPosition = originalBurgerPos;

        if (backgroundImage != null)
            backgroundImage.rectTransform.anchoredPosition = originalBackgroundPos;

        if (BoardImage != null)
            BoardImage.rectTransform.anchoredPosition = originalBoardPos;

        isShaking = false;
    }

}
