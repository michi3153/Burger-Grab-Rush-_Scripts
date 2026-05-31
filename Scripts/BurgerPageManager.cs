using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class BurgerData
{
    public string name;
    public List<string> ingredients = new List<string>();
    public int score;
    public Sprite image;

    public BurgerData(string name, List<string> ingredients, int score, Sprite image)
    {
        this.name = name;
        this.ingredients = ingredients;
        this.score = score;
        this.image = image;
    }
}
public class BurgerPageManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI pageIndicatorText;

    private int currentPage = 0;
    private List<BurgerData> burgerList = new List<BurgerData>();
    private int cardsPerPage= 3;

    public Sprite defaultBurgerSprite;
    public Sprite tomatoBurgerSprite;
    public Sprite CheeseBurgerSprite;
    public Sprite CheeTomaBurgerSprite;
    public Sprite ChickenBurgerSprite;
    public Sprite NiceBurgerSprite;
    public Sprite SappariBurgerSprite;
    public Sprite VegeBurgerSprite;
    public Sprite VolumeMaxBurgerSprite;

    AudioSource audioSource;
    public List<AudioClip> PageClip = new List<AudioClip>();

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        // 仮データ（本番はScriptableObjectなどから）
        burgerList.Add(new BurgerData("ハンバーガー", new List<string> { "バンズ（下）",  "パティ" ,"バンズ（上）"}, 50, defaultBurgerSprite));
        burgerList.Add(new BurgerData("トマトバーガー", new List<string> { "バンズ（下）","パティ","トマト", "バンズ（上）" }, 70,tomatoBurgerSprite));
        burgerList.Add(new BurgerData("チーズバーガー", new List<string> { "バンズ（下）",  "パティ","チーズ", "バンズ（上）" }, 70,CheeseBurgerSprite));
        burgerList.Add(new BurgerData("チキンフィレ", new List<string> { "バンズ（下）", "レタス","チキン", "バンズ（上）" }, 70,ChickenBurgerSprite));
        burgerList.Add(new BurgerData("フレッシュバーガー", new List<string> { "バンズ（下）", "レタス", "パティ", "トマト", "バンズ（上）" }, 100,NiceBurgerSprite));
        burgerList.Add(new BurgerData("チートマバーガー", new List<string> { "バンズ（下）", "パティ", "チーズ", "トマト", "バンズ（上）" }, 100,CheeTomaBurgerSprite));
        burgerList.Add(new BurgerData("ベジバーガー", new List<string> { "バンズ（下）", "レタス", "パティ", "トマト", "オニオン", "バンズ（上）" }, 100,VegeBurgerSprite));
        burgerList.Add(new BurgerData("さっぱりバーガー", new List<string> { "バンズ（下）", "レタス", "チキン", "チーズ", "オニオン", "バンズ（上）" }, 100,SappariBurgerSprite));
        burgerList.Add(new BurgerData("Maxバーガー", new List<string> { "バンズ（下）", "レタス", "チキン", "パティ", "チーズ", "トマト", "バンズ（上）" }, 150,VolumeMaxBurgerSprite));

        prevButton.onClick.AddListener(() => {
            PlayPageSound();
            ShowPrevPage();
        });
        nextButton.onClick.AddListener(() => {
            PlayPageSound();
            ShowNextPage();
        });

        ShowPage(0);
    }

    void ShowPage(int pageIndex)
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        int totalPages = Mathf.CeilToInt((float)burgerList.Count / cardsPerPage);
        currentPage = Mathf.Clamp(pageIndex, 0, totalPages - 1);

        int startIndex = currentPage * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, burgerList.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardParent);
            card.GetComponent<BurgerCardUI>().SetData(burgerList[i]);
        }

        pageIndicatorText.text = $"{currentPage + 1} / {totalPages}";

        prevButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(currentPage < totalPages - 1);
    }

    void PlayPageSound()
    {
        if (PageClip != null && PageClip.Count > 0)
        {
            audioSource.PlayOneShot(PageClip[0]);
        }
    }
    void ShowPrevPage() => ShowPage(currentPage - 1);
    void ShowNextPage() => ShowPage(currentPage + 1);
}
