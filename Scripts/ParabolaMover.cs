using UnityEngine;

public class ParabolaMover : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    public Transform trayTarget;
    private float height = 0.18f;     // 放物線の高さ
    private float duration = 1.0f;   // 飛ぶ時間
    private float elapsed = 0f;

    public void Initialize(Vector3 target, Transform tray)
    {
        Debug.Log("ParabolaMover.Initialize 呼び出し！ target: " + target);
        startPoint = transform.position;
        endPoint = target;
        trayTarget = tray;
    }

    void Update()
    {
        //Debug.Log("ParabolaMover.Update 実行中... t = " + elapsed);
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // 放物線補間（Parabola公式）
        Vector3 current = Vector3.Lerp(startPoint, endPoint, t);
        current.y += height * Mathf.Sin(Mathf.PI * t); // 山を作る
        transform.position = current;

        if (t >= 1f)
        {
            transform.SetParent(trayTarget);
            trayTarget.gameObject.AddComponent<TrayMover>(); // ← スクリプトで移動開始
            Destroy(this); // 自分はお役御免
        }
    }
}
