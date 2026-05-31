using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ToTitleHandler : MonoBehaviour, IPointerClickHandler
{
    AudioSource audioSource;
    public List<AudioClip> confirmClip = new List<AudioClip>();
    public string sceneName = "TitleScene";
    private bool isStarted = false;
    [SerializeField] private Button titleButton;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (titleButton != null)
        {
            titleButton.onClick.AddListener(SelectedToTitle);
        }
        else
        {
            Debug.LogWarning("[ToTitleHandler] titleButton がアサインされていません！");
        }
    }

    public void SelectedToTitle()
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ボタン押されました！");
    }

}
