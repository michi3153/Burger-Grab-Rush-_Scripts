using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Examples;
using System.Collections;
using System.Linq;
using static GameManager;
using Leap.PhysicalHands;

public class BurgerJudgeSystem : MonoBehaviour
{
    public Transform stackingZone;  // 積み重ねるゾーン
    public List<BurgerIngredientSpawner> ingredientSpawners;

    private Dictionary<string, List<string>> burgerTypes = new Dictionary<string, List<string>>();
    private List<string> currentBurgerTypes = new List<string>();

    public BurgerDisplayUI burgerDisplayUI;

    public BurgerSnapZoneCustom snapzone;
    public GameObject wrappedBurgerPrefab;
    public GameObject trayPrefab;
    private GameObject trayInstance; // トレーのインスタンス
    public Transform traySpawnPosition;
    public Transform popupPosition;

    public Transform tray;          // トレーの位置（着地点）

    public int CorrectCount;
    public int FailCount;

    public enum Difficulty { Easy, Normal, Hard}
    public Difficulty currentDifficulty;

    private Dictionary<Difficulty, Dictionary<string, int>> spawnRates = new Dictionary<Difficulty, Dictionary<string, int>>();

    private Dictionary<Difficulty, int> burgerSlotCountByDifficulty = new Dictionary<Difficulty, int>()
    {
        { Difficulty.Easy, 2 },
        { Difficulty.Normal, 2 },
        { Difficulty.Hard, 3 }
    };

    public static BurgerJudgeSystem Instance { get; private set; }

    [System.Serializable]
    public class BurgerOrder
    {
        public string burgerType;
        public float remainingTime; // 秒単位で更新
        public GameObject slotObject;
    }
    public List<BurgerOrder> currentOrders = new List<BurgerOrder>();
    public int orderlimit;

    public AudioClip judgeSE;
    public AudioClip resetSE;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //currentDifficulty = Difficulty.Hard;//初期か
        CorrectCount = 0;
        FailCount = 0;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        SpawnTray();
    }
    private void Start()
    {
        //ゲーム構造が完成したら外す
        var difficulty = GameManager.Instance.SelectedDifficulty;
        Debug.Log($"選択された難易度: {difficulty}");
        currentDifficulty = (Difficulty)difficulty;
        SetupBurgerTypes();
        SetupSpawnRates();
        BurgerUIManager.Instance.SetDifficulty(currentDifficulty);
        
        //StartCoroutine(InitAfterDelay());
    }

    private void SetupBurgerTypes()
    {
        burgerTypes["DefaultBurger"] = new List<string> { "Burger-Bread", "Burger-Patty-Chicken", "Burger-Bread-Top" };
        burgerTypes["TomatoBurger"] = new List<string> { "Burger-Bread", "Burger-Patty-Chicken", "Tomato-Slice", "Burger-Bread-Top" };
        burgerTypes["CheeseBurger"] = new List<string> { "Burger-Bread", "Burger-Patty-Chicken", "Cheese Thin", "Burger-Bread-Top" };
        burgerTypes["ChickenBurger"] = new List<string> { "Burger-Bread", "Lettuce", "Chicken-Burger", "Burger-Bread-Top" };
        burgerTypes["NiceBurger"] = new List<string> { "Burger-Bread", "Lettuce", "Burger-Patty-Chicken", "Tomato-Slice", "Burger-Bread-Top" };
        burgerTypes["CheeTomaBurger"] = new List<string> { "Burger-Bread", "Burger-Patty-Chicken", "Cheese Thin", "Tomato-Slice", "Burger-Bread-Top" };
        burgerTypes["VegeBurger"] = new List<string> { "Burger-Bread", "Lettuce", "Burger-Patty-Chicken", "Tomato-Slice", "Onion", "Burger-Bread-Top" };
        burgerTypes["SappariBurger"] = new List<string> { "Burger-Bread", "Lettuce", "Chicken-Burger", "Cheese Thin", "Onion", "Burger-Bread-Top" };
        burgerTypes["VolumeMaxBurger"] = new List<string> { "Burger-Bread", "Lettuce", "Chicken-Burger", "Burger-Patty-Chicken", "Cheese Thin", "Tomato-Slice", "Burger-Bread-Top" };
    }

    private void SetupSpawnRates()
    {
        // Easy
        spawnRates[Difficulty.Easy] = new Dictionary<string, int>()
        {
            { "DefaultBurger", 50 },
            { "TomatoBurger", 20 },
            { "CheeseBurger", 20 },
            { "ChickenBurger", 10 },
            { "NiceBurger", 0 },
            { "CheeTomaBurger", 0 },
            { "VegeBurger", 0 },
            { "SappariBurger", 0 },
            { "VolumeMaxBurger", 0 }
        };

        // Normal
        spawnRates[Difficulty.Normal] = new Dictionary<string, int>()
        {
            { "DefaultBurger", 0 },
            { "TomatoBurger", 10 },
            { "CheeseBurger", 30 },
            { "ChickenBurger", 30 },
            { "NiceBurger", 10 },
            { "CheeTomaBurger", 15 },
            { "VegeBurger", 5 },
            { "SappariBurger", 0 },
            { "VolumeMaxBurger", 0 }
        };

        // Hard
        spawnRates[Difficulty.Hard] = new Dictionary<string, int>()
        {
            { "DefaultBurger", 0 },
            { "TomatoBurger", 5 },
            { "CheeseBurger", 5 },
            { "ChickenBurger", 5 },
            { "NiceBurger", 30 },
            { "CheeTomaBurger", 30 },
            { "VegeBurger", 10 },
            { "SappariBurger", 10 },
            { "VolumeMaxBurger", 5 }
        };
    }

    public void JudgeBurger()
    {
        if(judgeSE != null &&audioSource != null)
        {
            audioSource.PlayOneShot(judgeSE);
        }
        List<Transform> stackedObjects = new List<Transform>();
        Debug.Log("[JudgeBurger] 開始");

        foreach (Transform child in stackingZone)
        {
            if (child.CompareTag("Snapped"))
            {
                stackedObjects.Add(child);
            }
        }

        stackedObjects.Sort((a, b) => a.position.y.CompareTo(b.position.y));

        List<string> collectedIngredients = new List<string>();
        foreach (Transform t in stackedObjects)
        {
            string cleanName = t.name.Replace("(Clone)", "").Trim();
            collectedIngredients.Add(cleanName);
        }
        Debug.Log($"[JudgeBurger] Collected ingredients: {string.Join(", ", collectedIngredients)}");
        //List<BurgerOrder> matchingOrders = new List<BurgerOrder>();
        Debug.Log($"[JudgeBurger] Current orders count: {currentOrders.Count}");
        for (int i = 0; i < currentOrders.Count; i++)
        {
            Debug.Log($"[Debug] Order {i + 1}: {currentOrders[i].burgerType}");
        }
        // 各バーガータイプに対して判定を行う
        var matchingOrders = currentOrders
            .Where(order => IsCorrect(collectedIngredients, order.burgerType))
            .OrderBy(order => order.remainingTime)
            .ToList();


        if (matchingOrders.Count > 0)
        {
            Debug.Log($"[JudgeBurger] 正解のオーダーが {matchingOrders.Count} 件ありました");
            BurgerOrder closestOrder = matchingOrders[0]; // 初期値として1番目の注文を選ぶ

            foreach (var order in matchingOrders)
            {
                if (order.remainingTime < closestOrder.remainingTime)
                {
                    closestOrder = order;  // 残り時間が少ない注文を更新
                }
            }

            Debug.Log($"{closestOrder.burgerType} が正しく作られました！");

            // UIスロットも正解扱いにする
            if (closestOrder.slotObject != null)
            {
                var timer = closestOrder.slotObject.GetComponent<BurgerTimer>();
                if (timer != null)
                {
                    BurgerUIManager.Instance.OnBurgerDeliveredCorrectly(timer);
                }
                else
                {
                    Debug.LogWarning($"[JudgeBurger] SlotObject に BurgerTimer が存在しません: {closestOrder.slotObject.name}");
                }
            }
        }
        else
        {
            Debug.Log("正しいバーガーが作られていません");
            FailCount++;
            ScoreManager.Instance.AddScore(-10);
            BurgerUIManager.Instance.TriggerFailureEffect();
        }

        Debug.Log($"Snapped count: {stackedObjects.Count}");

        DestroyAllIngredients();

        if (matchingOrders.Count > 0)
        {
            // ★バーガー演出処理を追加
            GameObject wrappedBurger = Instantiate(wrappedBurgerPrefab, snapzone.transform.position, Quaternion.identity);
            wrappedBurger.GetComponent<ParabolaMover>().Initialize(tray.position + new Vector3(0, 0.05f, 0), trayInstance.transform); // トレー上

            // ★必要に応じて遅延Destroy（バーガーが画面外に行ってから消したい場合）
            //Destroy(wrappedBurger, 3f); // アニメに合わせて調整
        }
        foreach (var spawner in ingredientSpawners)
        {
            spawner.SpawnIfNeeded();
        }

        if(snapzone != null)
        {
            snapzone.ResetStack();
        }
        Debug.Log($"[JudgeSystem] currentOrders.Count: {currentOrders.Count}");
        Debug.Log($"[JudgeSystem] UIの表示中のスロット数: {BurgerUIManager.Instance.GetSlotCount()}");

    }
    private bool IsCorrect(List<string> collected, string burgerType)
    {
        if (!burgerTypes.ContainsKey(burgerType))
        {
            Debug.LogWarning($"[IsCorrect] burgerType {burgerType} が辞書に存在しません");
            return false;
        }

        List<string> correctIngredients = burgerTypes[burgerType];

        if (collected.Count != correctIngredients.Count)
        {
            Debug.Log($"[IsCorrect] Count mismatch: Collected {collected.Count}, Expected {correctIngredients.Count}");
            return false;
        }

        for (int i = 0; i < correctIngredients.Count; i++)
        {
            if (collected[i] != correctIngredients[i])
            {
                Debug.Log($"[IsCorrect] Mismatch at {i}: {collected[i]} != {correctIngredients[i]}");
                return false;
            }
        }

        Debug.Log($"[IsCorrect] {burgerType} 正解！");
        return true;
    }


    private void DestroyAllIngredients()
    {
        List<GameObject> allIngredients = new List<GameObject>();
        allIngredients.AddRange(GameObject.FindGameObjectsWithTag("Ingredient"));
        allIngredients.AddRange(GameObject.FindGameObjectsWithTag("Snapped"));

        foreach (var obj in allIngredients)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null && GrabHelper.Instance != null)
            {
                bool isGrabbed = GrabHelper.Instance.IsObjectGrabbed(rb, out var _);
                if (isGrabbed)
                {
                    Debug.Log($"[DestroyAllIngredients] {obj.name} はつかまれているため破壊をスキップ");
                    continue; // つかまれていたら削除しない
                }
            }

            Destroy(obj);
        }
    }


    public void ResetBurger()
    {
        if(resetSE != null && audioSource != null)
        {
            audioSource.PlayOneShot(resetSE);
        }
        DestroyAllIngredients();

        foreach (var spawner in ingredientSpawners)
        {
            spawner.SpawnIfNeeded();
        }

        if (snapzone != null)
        {
            snapzone.ResetStack();
        }

        Debug.Log("リセットされました");

        // 現在のバーガーのUIはそのまま維持
    }

    private string WeightedRandom(List<string> items, List<int> weights)
    {
        int totalWeight = 0;
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        Debug.Log($"SelectedNumber: {randomValue}");
        int accumulatedWeight = 0;

        for (int i = 0; i < items.Count; i++)
        {
            accumulatedWeight += weights[i];
            if (randomValue < accumulatedWeight)
            {
                return items[i];
            }
        }

        return items[0]; // fallback
    }

    public List<string> GenerateBurgerQueue()
    {
        List<string> generated = new List<string>();
        var rates = spawnRates[currentDifficulty];
        int count = burgerSlotCountByDifficulty[currentDifficulty];

        List<string> keys = new List<string>();
        List<int> weights = new List<int>();

        foreach (var kvp in rates)
        {
            keys.Add(kvp.Key);
            weights.Add(kvp.Value);
        }

        for (int i = 0; i < count; i++)
        {
            string selected = WeightedRandom(keys, weights);
            generated.Add(selected);
        }

        return generated;
    }


    public string GetRandomBurgerKey()
    {
        var difficultyRates = spawnRates[currentDifficulty];
        int totalRate = 0;

        foreach (var pair in difficultyRates)
            totalRate += pair.Value;

        int rand = Random.Range(0, totalRate);
        int cumulative = 0;

        foreach (var pair in difficultyRates)
        {
            cumulative += pair.Value;
            if (rand < cumulative)
                return pair.Key;
        }

        return "DefaultBurger"; // 保険として
    }

    public void AddOrder(BurgerOrder order)
    {
        orderlimit = burgerSlotCountByDifficulty[currentDifficulty];
        if (currentOrders.Count >= orderlimit)
        {
            Debug.LogWarning($"[AddOrder] 注文数が上限({orderlimit})に達しています。{order.burgerType} は追加されません。");
            return;
        }
        Debug.Log($"[AddOrder] Adding order: {order.burgerType}. Total before add: {currentOrders.Count}");
        currentOrders.Add(order);
        Debug.Log($"[JudgeSystem] {order.burgerType} が currentOrders に追加されました");
    }

    public void RemoveOrder(BurgerOrder order)
    {
        if (currentOrders.Contains(order))
        {
            currentOrders.Remove(order);
            Debug.Log($"[JudgeSystem] {order.burgerType} を currentOrders から削除しました");
        }
    }

    public void RemoveOrderBySlot(GameObject slot)
    {
        Debug.Log("Trying to remove slot: " + slot.name);

        BurgerOrder orderToRemove = currentOrders.FirstOrDefault(o => o.slotObject == slot);
        if (orderToRemove != null)
        {
            Debug.Log("Removed order: " + orderToRemove.burgerType);
            currentOrders.Remove(orderToRemove);
        }
        else
        {
            Debug.LogWarning("RemoveOrderBySlot: Matching order not found for slot " + slot.name);
        }
        foreach (var order in currentOrders)
        {
            Debug.Log("Comparing slots: " + order.slotObject.name + " vs " + slot.name + " => " + (order.slotObject == slot));
        }
    }

    public string GetBurgerKeyBySlot(GameObject slot)
    {
        foreach (var order in currentOrders)
        {
            if (order.slotObject == slot)
            {
                return order.burgerType;
            }
        }

        Debug.LogWarning("[BurgerJudgeSystem] 該当スロットが見つかりませんでした");
        return null;
    }

    public void SpawnTray()
    {
        Debug.Log("新しいトレー出現！");
        trayInstance = Instantiate(trayPrefab, traySpawnPosition.position, Quaternion.identity);
    }
}
