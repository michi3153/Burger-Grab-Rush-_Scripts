using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using static RankingManager;

public class ResultUIManager : MonoBehaviour
{
    [Header("Result Texts")]
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private TextMeshProUGUI successFailText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Ranking Parents")]
    [SerializeField] private Transform easyRankingParent;
    [SerializeField] private Transform normalRankingParent;
    [SerializeField] private Transform hardRankingParent;
    [SerializeField] private Transform overallRankingParent;

    [SerializeField] private GameObject rankingEntryPrefab;

    private string currentSessionId = null;

    private void Start()
    {
        ShowResult();
        ShowRanking();
    }

    void ShowResult()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "--");
        pointText.text = $"Score: {GameManager.Instance.FinalScore}";
        successFailText.text = $"成功: {GameManager.Instance.SuccessCount}　失敗: {GameManager.Instance.FailCount}";
        resultText.text = $"結果({GameManager.Instance.SelectedDifficulty})";

        currentSessionId = RankingManager.SaveScore(
            GameManager.Instance.FinalScore,
            GameManager.Instance.SelectedDifficulty,
            playerName
        );
    }

    void ShowRanking()
    {
        // 全て非表示にしておく
        easyRankingParent.gameObject.SetActive(false);
        normalRankingParent.gameObject.SetActive(false);
        hardRankingParent.gameObject.SetActive(false);
        overallRankingParent.gameObject.SetActive(true);

        var selected = GameManager.Instance.SelectedDifficulty;
        int currentScore = GameManager.Instance.FinalScore;
        string playerName = PlayerPrefs.GetString("PlayerName", "--");

        // 難易度別のリスト取得
        var difficultyList = RankingManager.GetTopByDifficulty(selected);
        Transform targetParent = selected switch
        {
            GameManager.Difficulty.Easy => easyRankingParent,
            GameManager.Difficulty.Normal => normalRankingParent,
            GameManager.Difficulty.Hard => hardRankingParent,
            _ => null
        };

        if (targetParent != null)
        {
            targetParent.gameObject.SetActive(true);
            ShowRankList(targetParent, difficultyList, currentScore, playerName);
        }

        // 全体ランキング表示
        var overallList = RankingManager.GetTopOverall();
        ShowRankList(overallRankingParent, overallList, currentScore, playerName, true);
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

            // 自分のスコアならハイライト
            if (isCurrentPlayer)
            {
                entry.AddComponent<RankingGlow>();
            }
        }
    }

}
