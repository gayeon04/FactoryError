
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float strength = 30f;
    public float range = 4f;
    public float maxForce = 50f;

    public bool magnetEnabled = true;

    private const float epsilon = 0.1f;

    public void ApplyMagnet(PhysicsBody body)
    {
        if (!magnetEnabled)
            return;

        if (!body.TryGetComponent<Product>(
            out Product product))
            return;

        if (product.Data == null ||
            !product.Data.isMagnetic)
            return;

        Vector2 direction =
            (Vector2)transform.position
            - (Vector2)body.transform.position;

        float distanceSqr =
            direction.sqrMagnitude;

        // 영향 범위 검사
        if (distanceSqr > range * range)
            return;

        if (distanceSqr < 0.000001f)
            return;

        // 거리에 따른 힘 계산
        float forceMagnitude =
            strength / (distanceSqr + epsilon);

        forceMagnitude = Mathf.Min(
            forceMagnitude, maxForce
        );

        Vector2 force =
            direction.normalized * forceMagnitude;

        body.AddForce(force);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            range
        );
    }
}
