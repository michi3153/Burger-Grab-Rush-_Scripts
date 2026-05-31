using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    private const int MaxRank = 5;
    private static readonly string RankingKey = "RankingData";

    [System.Serializable]
    public class ScoreData
    {
        public int score;
        public GameManager.Difficulty difficulty;
        public string playerName = "NoName"; // 将来的に名前入力対応
        public string sessionId;
    }

    public static string SaveScore(int score, GameManager.Difficulty difficulty, string playerName, string sessionId = null)
    {
        var data = LoadRanking();

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "--";

        // セッションIDが空なら新規生成
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = System.Guid.NewGuid().ToString();
        }

        //data.RemoveAll(s => s.playerName == playerName && s.difficulty == difficulty);

        data.Add(new ScoreData { score = score, difficulty = difficulty, playerName = playerName, sessionId = sessionId});

        var trimmed = data.OrderByDescending(s => s.score).Take(100).ToList();
        string json = JsonUtility.ToJson(new ScoreListWrapper(trimmed));
        PlayerPrefs.SetString(RankingKey, json);
        PlayerPrefs.Save();

        return sessionId;
    }

    public static List<ScoreData> LoadRanking()
    {
        if (!PlayerPrefs.HasKey(RankingKey)) return new List<ScoreData>();
        string json = PlayerPrefs.GetString(RankingKey);
        return JsonUtility.FromJson<ScoreListWrapper>(json)?.scores ?? new List<ScoreData>();
    }

    public static List<ScoreData> GetTopByDifficulty(GameManager.Difficulty difficulty)
    {
        var scores = LoadRanking().Where(s => s.difficulty == difficulty)
                                  .OrderByDescending(s => s.score)
                                  .Take(MaxRank)
                                  .ToList();

        while (scores.Count < MaxRank)
        {
            scores.Add(new ScoreData { score = 0, playerName = "--", difficulty = difficulty });
        }

        return scores;
    }

    public static List<ScoreData> GetTopOverall()
    {
        var scores = LoadRanking()
        .OrderByDescending(s => s.score)
        .Take(5)
        .ToList();

        while (scores.Count < MaxRank)
        {
            scores.Add(new ScoreData { score = 0, playerName = "--", difficulty = GameManager.Difficulty.Easy });
        }

        return scores;
    }

    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<ScoreData> scores;
        public ScoreListWrapper(List<ScoreData> scores) => this.scores = scores;
    }

    public static void CheckVersionAndClearIfNeeded()
    {
        string currentVersion = Application.version;
        string savedVersion = PlayerPrefs.GetString("SavedGameVersion", "");

        if (savedVersion != currentVersion)
        {
            Debug.Log("新しいバージョンを検出。ランキングを初期化します。");

            // 🔻 ランキングデータを削除
            PlayerPrefs.DeleteKey(RankingKey);

            // 🔻 今回のバージョンを保存
            PlayerPrefs.SetString("SavedGameVersion", currentVersion);
            PlayerPrefs.Save();
        }
    }

}
