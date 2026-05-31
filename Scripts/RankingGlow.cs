using UnityEngine;
using UnityEngine.UI;

public class RankingGlow : MonoBehaviour
{
    private Image background;
    private Color baseColor;
    private float timer;

    void Start()
    {
        background = GetComponent<Image>();
        if (background != null)
        {
            // ShowRankList で設定された色をそのまま基準色とする
            baseColor = background.color;
        }
        else
        {
            Debug.LogWarning("RankingGlow: Image component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (background == null) return;

        timer += Time.deltaTime;
        float brightness = 0.85f + Mathf.Sin(timer * 3f) * 0.15f; // 0.6〜1.0程度で明滅

        // RGB値を明るく（アルファはそのまま）
        Color glowColor = baseColor * brightness;
        glowColor.a = baseColor.a; // アルファは固定（透けない）

        background.color = glowColor;
    }
}
