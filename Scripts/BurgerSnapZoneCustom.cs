using System.Collections.Generic;
using UnityEngine;
using Leap.PhysicalHands;

public class BurgerSnapZoneCustom : MonoBehaviour
{
    public Transform stackingCenter;
    public float snapRadius = 0.09f;
    public float stackingOffsetY = 0.05f;
    public BoxCollider snapCollider;

    private float currentStackHeight = 0f;
    private List<Rigidbody> snappingObjects = new List<Rigidbody>();
    private Dictionary<Rigidbody, Vector3> snapTargets = new Dictionary<Rigidbody, Vector3>();

    private void Awake()
    {
        // Inspector でアタッチされていなかった場合のみ自動取得
        if (snapCollider == null)
        {
            snapCollider = GetComponent<BoxCollider>();
        }
    }

    private void Start()
    {
        if (snapCollider == null)
        {
            snapCollider = gameObject.AddComponent<BoxCollider>();
            snapCollider.isTrigger = true;
            snapCollider.center = Vector3.zero;
            snapCollider.size = new Vector3(snapRadius * 2, 1f, snapRadius * 2);
        }
    }

    private void FixedUpdate()
    {
        foreach (var rb in FindObjectsOfType<Rigidbody>())
        {
            if (GrabHelper.Instance == null) continue;

            bool isGrabbed = GrabHelper.Instance.IsObjectGrabbed(rb, out var _);

            // 👇 掴んでるなら GrabbingObject に変更
            if (isGrabbed && rb.gameObject.layer != LayerMask.NameToLayer("GrabbingObject"))
            {
                rb.gameObject.layer = LayerMask.NameToLayer("GrabbingObject");
            }

            // 👇 掴んでない＋Snap判定内ならスナップ処理
            if (!isGrabbed &&
                rb.gameObject.activeInHierarchy &&
                !rb.CompareTag("Snapped") &&
                IsWithinSnapZone(rb) &&
                !snappingObjects.Contains(rb))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.transform.SetParent(stackingCenter);
                Vector3 targetPos = stackingCenter.position + new Vector3(0, currentStackHeight + 0.04f, 0);
                snappingObjects.Add(rb);
                snapTargets[rb] = targetPos;

                float height = GetObjectHeight(rb);
                currentStackHeight += height;

                rb.gameObject.tag = "Snapped";
                rb.gameObject.layer = LayerMask.NameToLayer("SnappedObject");

                var ignoreHands = rb.GetComponent<IgnorePhysicalHands>();
                if (ignoreHands != null)
                {
                    ignoreHands.enabled = true;
                    ignoreHands.DisableAllGrabbing = false;
                    ignoreHands.DisableAllHandCollisions = true;
                }

                continue;
            }

            if (isGrabbed && rb.gameObject.layer != LayerMask.NameToLayer("GrabbingObject"))
            {
                rb.gameObject.layer = LayerMask.NameToLayer("GrabbingObject");
            }

            // 👇 掴んでなくて Snapped じゃなくて範囲外にある → Layer 戻す
            if (!isGrabbed && rb.gameObject.layer == LayerMask.NameToLayer("GrabbingObject") && rb.tag != "Snapped")
            {
                rb.gameObject.layer = LayerMask.NameToLayer("Ingredients"); // または適切なレイヤー
            }
        }

        for (int i = snappingObjects.Count - 1; i >= 0; i--)
        {
            var rb = snappingObjects[i];
            if (rb == null) continue;

            Vector3 target = snapTargets[rb];
            Vector3 newPos = Vector3.Lerp(rb.position, target, 0.15f);
            rb.MovePosition(newPos);

            if (Vector3.Distance(rb.position, target) < 0.001f)
            {
                rb.position = target;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.isKinematic = true;

                snappingObjects.RemoveAt(i);
                snapTargets.Remove(rb);
            }
        }
    }

    private bool IsWithinSnapZone(Rigidbody rb)
    {
        return snapCollider.bounds.Contains(rb.position);
    }

    private float GetObjectHeight(Rigidbody rb)
    {
        var renderer = rb.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y;
        }

        var collider = rb.GetComponentInChildren<Collider>();
        if (collider != null)
        {
            return collider.bounds.size.y;
        }

        return stackingOffsetY;
    }

    private void OnDrawGizmos()
    {
        if (stackingCenter == null) return;

        Vector3 center = stackingCenter.position;
        float half = snapRadius;
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(center + new Vector3(-half, 0, -half), center + new Vector3(-half, 0, half));
        Gizmos.DrawLine(center + new Vector3(-half, 0, half), center + new Vector3(half, 0, half));
        Gizmos.DrawLine(center + new Vector3(half, 0, half), center + new Vector3(half, 0, -half));
        Gizmos.DrawLine(center + new Vector3(half, 0, -half), center + new Vector3(-half, 0, -half));
    }

    public void ResetStack()
    {
        currentStackHeight = 0f;

        // 親から子を解除し、元に戻す（必要に応じて）
        foreach (Transform child in stackingCenter)
        {
            if (child.CompareTag("Snapped"))
            {
                child.SetParent(null);
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.gameObject.tag = "Untagged";
                    rb.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }

        snappingObjects.Clear();
        snapTargets.Clear();
    }

}
