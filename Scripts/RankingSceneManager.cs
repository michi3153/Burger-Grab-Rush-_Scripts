using UnityEngine;
using UnityEngine.UI;

public class RankingSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject rankingParentSet1; // Easy + Normal
    [SerializeField] private GameObject rankingParentSet2; // Hard + Total
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private AudioClip clickSound; // 効果音
    private AudioSource audioSource;

    private int currentPage = 0;

    void Start()
    {
        // 最初は1ページ目のみ表示
        //ShowPage(0);

        audioSource = GetComponent<AudioSource>();

        nextButton.onClick.AddListener(() => {
            PlayClickSound();
            ShowPage(1);
        });

        prevButton.onClick.AddListener(() => {
            PlayClickSound();
            ShowPage(0);
        });
    }

    public void Initialize()
    {
        ShowPage(0);
    }

    void ShowPage(int page)
    {
        currentPage = page;

        bool isFirstPage = currentPage == 0;

        rankingParentSet1.SetActive(isFirstPage); // Easy + Normal
        rankingParentSet2.SetActive(!isFirstPage); // Hard + Total

        prevButton.gameObject.SetActive(!isFirstPage);
        nextButton.gameObject.SetActive(isFirstPage);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
