using UnityEngine;
using System.Collections;

public class TrayAnimatorHelper : MonoBehaviour
{
    public GameObject trayPrefab;
    public Transform spawnPosition;
    public float respawnDelay = 1f; // 任意のディレイ時間（秒）
    public BurgerJudgeSystem judgeSystem;

    public void OnTrayAnimationEnd()
    {
        Debug.Log("アニメ終わった！");
        Destroy(this.gameObject); // トレー（と子バーガー）を削除
        StartCoroutine(RespawnTrayAfterDelay());
    }

    private IEnumerator RespawnTrayAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (judgeSystem != null)
        {
            judgeSystem.SpawnTray();
        }
    }
}
