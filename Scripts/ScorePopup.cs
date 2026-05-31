using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public TMP_Text popupText;
    public float floatUpSpeed = 30f;
    public float fadeOutSpeed = 1f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(int amount)
    {
        popupText.text = (amount > 0 ? "+" : "") + amount.ToString();
        popupText.color = amount > 0 ? Color.green : Color.red;
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * floatUpSpeed * Time.deltaTime;
        canvasGroup.alpha -= fadeOutSpeed * Time.deltaTime;
    }
}
