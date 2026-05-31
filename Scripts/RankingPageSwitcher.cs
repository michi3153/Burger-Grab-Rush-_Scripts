
using UnityEngine;


public class RankingPageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;

    private int currentPage = 0;

    private void Start()
    {
        ShowPage(0); // 最初は1ページ目を表示
    }

    public void ShowPage(int page)
    {
        page1.SetActive(page == 0);
        page2.SetActive(page == 1);
        currentPage = page;
    }

    public void NextPage()
    {
        ShowPage((currentPage + 1) % 2);
    }

    public void PrevPage()
    {
        ShowPage((currentPage + 1) % 2);
    }
}