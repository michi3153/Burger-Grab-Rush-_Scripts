using UnityEngine;

public class TrayMover : MonoBehaviour
{
    public float speed = 0.5f;
    public float destroyX = -1f; // 画面外に出るX座標（必要に応じて調整）

    private bool respawned = false;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (!respawned && transform.position.x <= destroyX)
        {
            respawned = true;
            FindObjectOfType<BurgerJudgeSystem>().SpawnTray(); // JudgeSystemにトレー再生成を依頼
            Destroy(gameObject);
        }
    }
}
