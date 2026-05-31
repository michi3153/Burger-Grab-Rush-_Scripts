using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int FinalScore {get; set;}
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public enum Difficulty { Easy, Normal, Hard }
    public enum GameState { Ready, Playing, Ended }
    public GameState CurrentGameState { get; private set; }
    public Difficulty SelectedDifficulty { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // ÅöÅö Ç±ÇÍÇ™Ç»Ç¢Ç∆ê‚ëŒè¡Ç¶ÇÈ
        }
        else
        {
            Destroy(gameObject); // èdï°ñhé~
        }
    }

    private void Start()
    {
        RankingManager.CheckVersionAndClearIfNeeded();
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        SelectedDifficulty = difficulty;
    }

    public void SetGameState(GameState state)
    {
        CurrentGameState = state;
    }
}


