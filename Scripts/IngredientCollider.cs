using UnityEngine;

namespace Leap.Examples
{
    public class IngredientCollider : MonoBehaviour
    {
        private Collider tableCollider;  // テーブルのCollider

        private void Start()
        {
            // テーブルのColliderを自動で取得
            tableCollider = GameObject.Find("Table-up").GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // 衝突したオブジェクトがテーブルと当たったら破棄
            if (other == tableCollider)
            {
                Destroy(gameObject);  // オブジェクト自体を破棄
            }
        }
    }
}
