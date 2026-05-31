using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int currentScore { get; private set; }
    public TMP_Text scoreText;

    public GameObject scorePopupPrefab;
    public Transform popupParent; // 通常は Canvas 直下

    // ★ バーガーごとの点数辞書
    private Dictionary<string, int> burgerScores = new Dictionary<string, int>()
    {
        { "DefaultBurger", 50 },
        { "TomatoBurger", 70 },
        { "CheeseBurger", 70 },
        { "ChickenBurger", 70 },
        { "NiceBurger",  100},
        { "CheeTomaBurger", 100 },
        { "VegeBurger", 100 },
        { "SappariBurger", 100 },
        { "VolumeMaxBurger", 150 }
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        currentScore = 0;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    // ★ バーガー名から得点加算
    public int AddScoreByBurgerName(string burgerName)
    {
        if (burgerScores.ContainsKey(burgerName))
        {
            AddScore(burgerScores[burgerName]);
            return burgerScores[burgerName];
        }
        else
        {
            Debug.LogWarning($"[ScoreManager] 不明なバーガー名: {burgerName}");
            AddScore(0);
            return 0;
        }
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "スコア: " + currentScore;
    }

    public void ShowScorePopup(int amount, Vector3 screenPosition)
    {
        GameObject popup = Instantiate(scorePopupPrefab, popupParent);
        popup.transform.position = screenPosition;
        popup.GetComponent<ScorePopup>().Show(amount);
    }
}
