using UnityEngine;
using Leap.Examples;

public class IngredientAutoReset : MonoBehaviour
{
    public Transform snapZoneCenter; // SnapZoneの中心Transform
    public float maxDistance = 1.5f;   // SnapZoneからの最大許容距離
    private float maxDistanceSqr;
    public BurgerIngredientSpawner mySpawner;

    private void Start()
    {
        maxDistanceSqr = maxDistance * maxDistance;
    }
    private void Update()
    {
        if (snapZoneCenter == null) return;

        //zx平面の距離(縦方向は積み重ねるときに十分離れる可能性)
        Vector3 offset = transform.position - snapZoneCenter.position;
        float distSqr = offset.sqrMagnitude;
        if (distSqr > maxDistanceSqr)
        {
            Debug.Log($"{gameObject.name} がSnapZoneから {distSqr:F2} 離れたのでリセット");
            Destroy(gameObject);

            if (mySpawner != null)
            {
                mySpawner.SpawnIfNeeded();
            }
        }
    }
}
