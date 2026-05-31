using UnityEngine;
using TMPro; // TextMeshPro‚ğg‚Á‚Ä‚éê‡‚Ì‚İ

public class TextScaler : MonoBehaviour
{
    public float scaleAmplitude = 0.01f; // Šg‘åk¬‚Ì•
    public float scaleSpeed = 1.0f;     // ‘¬‚³i¬‚³‚¢‚Ù‚Ç‚ä‚Á‚­‚èj
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
        transform.localScale = initialScale * scaleFactor;
    }
}
