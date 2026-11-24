
using UnityEngine;

public class PhysicsBox : MonoBehaviour
{
    [Header("Collision Properties")]
    public Vector2 size = Vector2.one;

    [Range(0f, 1f)]
    public float restitution = 0.5f;

    public Vector2 Center =>
        transform.position;

    public Vector2 AxisX =>
        ((Vector2)transform.right).normalized;

    public Vector2 AxisY =>
        ((Vector2)transform.up).normalized;

    public Vector2 HalfSize
    {
        get
        {
            Vector3 scale = transform.lossyScale;

            return new Vector2(
                Mathf.Abs(size.x * scale.x) * 0.5f,
                Mathf.Abs(size.y * scale.y) * 0.5f
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(
            Vector3.zero,
            (Vector3)(HalfSize * 2f)
        );

        Gizmos.matrix = Matrix4x4.identity;
    }
}
