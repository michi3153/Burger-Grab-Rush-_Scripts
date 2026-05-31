using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using static RankingManager;

public class RankingUIManager : MonoBehaviour
{
    [Header("Ranking Parents")]
    [SerializeField] private Transform easyRankingParent;
    [SerializeField] private Transform normalRankingParent;
    [SerializeField] private Transform hardRankingParent;
    [SerializeField] private Transform overallRankingParent;

    [SerializeField] private GameObject rankingEntryPrefab;

    private string currentSessionId = null;

    private void Start()
    {
        ShowRanking();
    }
    void ShowRanking()
    {
        // 全て非表示にしておく
        easyRankingParent.gameObject.SetActive(true);
        normalRankingParent.gameObject.SetActive(true);
        hardRankingParent.gameObject.SetActive(true);
        overallRankingParent.gameObject.SetActive(true);

        var selected = GameManager.Instance.SelectedDifficulty;
        int currentScore = GameManager.Instance.FinalScore;
        string playerName = PlayerPrefs.GetString("PlayerName", "--");

        ShowRankList(easyRankingParent, RankingManager.GetTopByDifficulty(GameManager.Difficulty.Easy), currentScore, playerName);
        ShowRankList(normalRankingParent, RankingManager.GetTopByDifficulty(GameManager.Difficulty.Normal), currentScore, playerName);
        ShowRankList(hardRankingParent, RankingManager.GetTopByDifficulty(GameManager.Difficulty.Hard), currentScore, playerName);
        ShowRankList(overallRankingParent, RankingManager.GetTopOverall(), currentScore, playerName, true);

        FindObjectOfType<RankingSceneManager>()?.Initialize();

    }

    void ShowRankList(Transform parent, List<ScoreData> scores, int currentScore, string currentName, bool isOverall = false)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        for (int i = 0; i < scores.Count; i++)
        {
            var data = scores[i];
            var entry = Instantiate(rankingEntryPrefab, parent);

            // Textオブジェクトを個別に取得
            var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            TextMeshProUGUI rankText = null;
            TextMeshProUGUI nameText = null;
            TextMeshProUGUI scoreText = null;

            foreach (var tmp in texts)
            {
                if (tmp.name.Contains("Rank")) rankText = tmp;
                else if (tmp.name.Contains("Name")) nameText = tmp;
                else if (tmp.name.Contains("Score")) scoreText = tmp;
            }

            if (rankText != null) rankText.text = $"{(i + 1)}位".ToString();
            if (nameText != null) nameText.text = data.playerName;
            if (scoreText != null) scoreText.text = data.score.ToString();

            var background = entry.GetComponent<Image>();

            // 自分のスコアかどうか判定
            bool isCurrentPlayer = data.sessionId == currentSessionId;

            if (isOverall)
            {
                // 全体ランキング：難易度で色分け
                switch (data.difficulty)
                {
                    case GameManager.Difficulty.Easy:
                        background.color = new Color32(0, 255, 0, 150); // 緑
                        break;
                    case GameManager.Difficulty.Normal:
                        background.color = new Color32(255, 255, 0, 150); // 黄
                        break;
                    case GameManager.Difficulty.Hard:
                        background.color = new Color32(255, 0, 0, 150); // 赤
                        break;
                }
            }
            else
            {
                // 難易度別：順位で色分け
                if (i == 0) background.color = new Color32(255, 215, 0, 150);        // 金
                else if (i == 1) background.color = new Color32(192, 192, 192, 150); // 銀
                else if (i == 2) background.color = new Color32(205, 127, 50, 150);  // 銅
                else background.color = new Color32(169, 169, 169, 150);             // グレー
            }
        }
    }

}
