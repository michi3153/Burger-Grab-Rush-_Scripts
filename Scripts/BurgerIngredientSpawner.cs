using UnityEngine;

namespace Leap.Examples
{
    public class BurgerIngredientSpawner : MonoBehaviour
    {
        public Transform objectToSpawn; // 材料Prefab
        public Transform spawnPoint;    // スポーンする位置
        public Transform snapZoneCenter;
        private Transform currentSpawnedObject; // 今スポーンしているオブジェクト

        private void Start()
        {
            SpawnIfNeeded();
        }

        private void Update()
        {
            // スポーンしたオブジェクトが消えたら、自動で再スポーン
            if (currentSpawnedObject == null)
            {
                SpawnIfNeeded();
            }
        }

        public void SpawnIfNeeded()
        {
            if (objectToSpawn == null || spawnPoint == null)
            {
                Debug.LogWarning("Spawn設定が足りません！");
                return;
            }

            // 新しく生成
            Transform newObj = Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);
            currentSpawnedObject = newObj;

            IngredientAutoReset resetScript = newObj.GetComponent<IngredientAutoReset>();
            if (resetScript != null)
            {
                resetScript.snapZoneCenter = snapZoneCenter;
                resetScript.mySpawner = this;
            }
        }
    }
}
