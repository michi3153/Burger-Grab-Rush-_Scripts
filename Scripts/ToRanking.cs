using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToRanking : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> confirmClip = new List<AudioClip>();
    public string sceneName = "RankingScene";
    private bool isStarted = false;
    [SerializeField] private Button ingredientButton;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if (ingredientButton != null)
        {
            ingredientButton.onClick.AddListener(SelectedToRanking);
        }
        else
        {
            Debug.LogWarning("[ToIngredientHandler] ingredientButton がアサインされていません！");
        }
    }

    public void SelectedToRanking()
    {
        if (isStarted) return;
        isStarted = true;
        if (confirmClip.Count > 0 && confirmClip[0] != null)
        {
            audioSource.PlayOneShot(confirmClip[0]);
            Debug.Log("[GameStartHandler] 効果音再生開始");
        }
        else
        {
            Debug.LogWarning("[GameStartHandler] ConfirmClip が設定されていません！");
        }

        Invoke(nameof(LoadTitleScene), 0.5f);
    }

    private void LoadTitleScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
