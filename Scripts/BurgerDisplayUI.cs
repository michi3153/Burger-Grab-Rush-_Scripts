using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerDisplayUI : MonoBehaviour
{
    public Sprite defaultBurgerSprite;
    public Sprite tomatoBurgerSprite;
    public Sprite CheeseBurgerSprite;
    public Sprite CheeTomaBurgerSprite;
    public Sprite ChickenBurgerSprite;
    public Sprite NiceBurgerSprite;
    public Sprite SappariBurgerSprite;
    public Sprite VegeBurgerSprite;
    public Sprite VolumeMaxBurgerSprite;

    private Dictionary<string, Sprite> burgerImages = new Dictionary<string, Sprite>();

    public Transform orderSlotParent;
    public GameObject slotPrefab;

    public bool isCompleted;

    public IReadOnlyDictionary<string, Sprite> BurgerImages => burgerImages;  // 他から参照専用

    private void Awake()  // Startより早い（Managerからすぐ使えるように）
    {
        burgerImages["DefaultBurger"] = defaultBurgerSprite;
        burgerImages["TomatoBurger"] = tomatoBurgerSprite;
        burgerImages["CheeseBurger"] = CheeseBurgerSprite;
        burgerImages["CheeTomaBurger"] = CheeTomaBurgerSprite;
        burgerImages["ChickenBurger"] = ChickenBurgerSprite;
        burgerImages["NiceBurger"] = NiceBurgerSprite;
        burgerImages["SappariBurger"] = SappariBurgerSprite;
        burgerImages["VegeBurger"] = VegeBurgerSprite;
        burgerImages["VolumeMaxBurger"] = VolumeMaxBurgerSprite;
    }

    // BurgerDisplayUI.cs に追加
    public void DisplayOrders(List<BurgerJudgeSystem.BurgerOrder> orders)
    {
        foreach (Transform child in orderSlotParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var order in orders)
        {
            GameObject newSlot = Instantiate(slotPrefab, orderSlotParent);
            Image burgerImage = newSlot.GetComponentInChildren<Image>();
            if (burgerImages.ContainsKey(order.burgerType))
            {
                burgerImage.sprite = burgerImages[order.burgerType];
            }
        }
    }


}
