using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStartFlow : MonoBehaviour
{
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject goText;
    [SerializeField] private float readyTime = 1f;
    [SerializeField] private float goTime = 0.5f;

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Ready);

        readyText.SetActive(true);
        yield return new WaitForSeconds(readyTime);
        readyText.SetActive(false);

        goText.SetActive(true);
        yield return new WaitForSeconds(goTime);
        goText.SetActive(false);

        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        Debug.Log("[GameStartFlow] ゲーム開始！");

        BurgerUIManager.Instance.SetupGame();
        // 必要ならここで StartGame() などを呼んでもOK
    }
}
