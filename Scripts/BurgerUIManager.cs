using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BurgerJudgeSystem;
using static GameManager;

public class BurgerUIManager : MonoBehaviour
{
    public Transform slotParent;
    public GameObject slotPrefab;
    public BurgerDisplayUI displayUI;
    public BurgerJudgeSystem judgeSystem;
    public GameObject scorePopupPrefab;
    public Transform popupParent;

    public float slotTimerDuration = 20f;
    //public float spawnDelay = 3f;  // 即時に近くするならここを短くしてOK

    private List<GameObject> activeSlots = new List<GameObject>();
    private string backOrder;
    private Dictionary<BurgerJudgeSystem.Difficulty, int> burgerSlotCountByDifficulty = new Dictionary<BurgerJudgeSystem.Difficulty, int>()
    {
        { BurgerJudgeSystem.Difficulty.Easy, 2 },
        { BurgerJudgeSystem.Difficulty.Normal, 2 },
        { BurgerJudgeSystem.Difficulty.Hard, 3 }
    };

    private Dictionary<BurgerJudgeSystem.Difficulty, float> spawnInitialDelayByDifficulty = new Dictionary<BurgerJudgeSystem.Difficulty, float>()
    {
        { BurgerJudgeSystem.Difficulty.Easy, 3.0f },
        { BurgerJudgeSystem.Difficulty.Normal, 3.0f },
        { BurgerJudgeSystem.Difficulty.Hard, 4.0f }
    };

    private Dictionary<BurgerJudgeSystem.Difficulty, float> spawnDelayByDifficulty = new Dictionary<BurgerJudgeSystem.Difficulty, float>()
    {
        { BurgerJudgeSystem.Difficulty.Easy, 3.0f },
        { BurgerJudgeSystem.Difficulty.Normal, 2.5f },
        { BurgerJudgeSystem.Difficulty.Hard, 2.0f }
    };

    private BurgerJudgeSystem.Difficulty currentDifficulty;
    public static BurgerUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetDifficulty(BurgerJudgeSystem.Difficulty difficulty)
    {
        currentDifficulty = (BurgerJudgeSystem.Difficulty)difficulty;
    }

    public void SetupGame()
    {
        ClearAllSlots();

        int frontCount = burgerSlotCountByDifficulty[currentDifficulty];

        StartCoroutine(SpawnInitialBurgers(frontCount));
    }

    private IEnumerator SpawnInitialBurgers(int count)
    {
        backOrder = BurgerJudgeSystem.Instance.GetRandomBurgerKey();
        for (int i = 0; i < count; i++)
        {
            backOrder = BurgerJudgeSystem.Instance.GetRandomBurgerKey();
            string key = BurgerJudgeSystem.Instance.GetRandomBurgerKey();
            CreateBurgerSlot(key);

            yield return new WaitForSeconds(spawnInitialDelayByDifficulty[currentDifficulty]); // ← 遅延追加
        }

        //backOrder = BurgerJudgeSystem.Instance.GetRandomBurgerKey();
    }

    public void OnBurgerTimeOut(BurgerTimer timer)
    {
        Debug.Log("[OnBurgerTimeOut] バーガータイムアップ");

        ScoreManager.Instance.AddScore(-20);
        ShowScorePopup(-20, new Vector2(0f, 0f));
        judgeSystem.FailCount++;

        StartCoroutine(TimeupEffect(timer.gameObject));
    }

    public void OnBurgerDeliveredCorrectly(BurgerTimer timer)
    {
        Debug.Log("[OnBurgerDeliveredCorrectly] 正しいバーガー提供");
        string burgerKey = BurgerJudgeSystem.Instance.GetBurgerKeyBySlot(timer.gameObject);
        int score = ScoreManager.Instance.AddScoreByBurgerName(burgerKey);
        ShowScorePopup(score, new Vector3(0f, 0f));
        if (timer.fillImage.color == timer.greenColor)
        {
            int bonusScore = 30;
            ScoreManager.Instance.AddScore(bonusScore);

            Vector3 offset = new Vector3(0f, 20f, 0f);
            ShowScorePopup(bonusScore, new Vector2(-50f, 25f));
        }
        judgeSystem.CorrectCount++;

        StartCoroutine(SuccessEffect(timer.gameObject));  // ← 新しい演出付きに変更
    }


    public IEnumerator SpawnNextBurger()
    {
        yield return new WaitForSeconds(spawnDelayByDifficulty[currentDifficulty]);

        int maxSlots = burgerSlotCountByDifficulty[currentDifficulty];
        if (activeSlots.Count < maxSlots)
        {
            // 裏を使って補充
            CreateBurgerSlot(backOrder);

            // 裏を新しく補充
            backOrder = BurgerJudgeSystem.Instance.GetRandomBurgerKey();
        }
    }

    private void CreateBurgerSlot(string burgerKey)
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing) return;

        GameObject slot = Instantiate(slotPrefab, slotParent);
        var timer = slot.GetComponent<BurgerTimer>();

        Image img = slot.transform.Find("Content/BurgerImage").GetComponent<Image>();
        if (displayUI.BurgerImages.ContainsKey(burgerKey))
        {
            img.sprite = displayUI.BurgerImages[burgerKey];
        }
        else
        {
            img.sprite = displayUI.defaultBurgerSprite;
            Debug.LogWarning($"Unknown burger key: {burgerKey}");
        }

        timer.totalTime = slotTimerDuration;
        timer.StartTimer();
        timer.OnTimeOut += OnBurgerTimeOut;

        activeSlots.Add(slot);

        BurgerOrder order = new BurgerOrder
        {
            burgerType = burgerKey,
            remainingTime = slotTimerDuration,
            slotObject = slot
        };
        BurgerJudgeSystem.Instance.AddOrder(order);
    }

    public void RemoveSlot(GameObject slot)
    {
        if (activeSlots.Contains(slot))
        {
            activeSlots.Remove(slot);
        }
        Destroy(slot);
    }

    public void ClearAllSlots()
    {
        foreach (var slot in activeSlots)
        {
            if (slot != null)
                Destroy(slot);
        }

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        activeSlots.Clear();
        backOrder = null;
    }

    public int GetSlotCount()
    {
        return activeSlots.Count;
    }

    public IEnumerator SuccessEffect(GameObject slot)
    {
        Image image = slot.transform.Find("Content/BurgerImage").GetComponent<Image>();
        Color originalColor = image.color;

        image.color = Color.green;

        yield return new WaitForSeconds(1f);

        if (image != null)
        {
            image.color = originalColor;
        }

        // 削除（既存処理と重複しないよう注意）
        RemoveSlot(slot);
        BurgerJudgeSystem.Instance.RemoveOrderBySlot(slot);

        StartCoroutine(SpawnNextBurger());
    }

    public IEnumerator TimeupEffect(GameObject slot)
    {
        Image image = slot.transform.Find("Content/BurgerImage").GetComponent<Image>();
        Color originalColor = image.color;

        image.color = Color.red;

        yield return new WaitForSeconds(1f);

        if (image != null)
        {
            image.color = originalColor;
        }

        // 削除（既存処理と重複しないよう注意）
        BurgerJudgeSystem.Instance.RemoveOrderBySlot(slot);
        RemoveSlot(slot);

        StartCoroutine(SpawnNextBurger());
    }

    public IEnumerator FailureEffect()
    {
        ShowScorePopup(-10, new Vector2(0f, 0f));
        foreach (var slot in activeSlots)
        {
            StartCoroutine(BlinkRed(slot));
        }

        yield return new WaitForSeconds(0.6f);
    }

    private IEnumerator BlinkRed(GameObject slot)
    {
        Image image = slot.transform.Find("Content/BurgerImage").GetComponent<Image>();
        Color originalColor = image.color;

        for (int i = 0; i < 2; i++)
        {
            image.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            image.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TriggerFailureEffect()
    {
        StartCoroutine(FailureEffect());
    }

    public void ShowScorePopup(int amount, Vector2 anchoredOffset)
    {
        GameObject popup = Instantiate(scorePopupPrefab, popupParent);

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = anchoredOffset; // ← UIローカル座標で指定

        var popupScript = popup.GetComponent<ScorePopup>();
        popupScript.Show(amount);
    }

}
