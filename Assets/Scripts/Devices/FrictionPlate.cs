
using UnityEngine;

[RequireComponent(typeof(PhysicsBox))]
public class FrictionPlate : MonoBehaviour
{
    [Range(0f, 1f)]
    public float friction = 0.5f;

    private PhysicsBox physicsBox;

    private void Awake()
    {
        physicsBox = GetComponent<PhysicsBox>();
    }

    public void ApplyFriction(
        PhysicsBody body,
        CollisionInfo collision,
        Vector2 gravity,
        float dt)
    {
        Vector2 normal = collision.normal;

        // 윗면 접촉 검사
        if (Vector2.Dot(
            normal, physicsBox.AxisY) < 0.95f)
            return;

        Vector2 tangent = physicsBox.AxisX;

        float tangentSpeed = Vector2.Dot(
            body.velocity, tangent
        );

        // 중력의 법선 방향 성분
        float normalAcceleration =
            Mathf.Max(
                0f,
                -Vector2.Dot(gravity, normal)
            );

        // 마찰에 의한 감속도
        float frictionAcceleration =
            friction * normalAcceleration;

        float newSpeed = Mathf.MoveTowards(
            tangentSpeed,
            0f,
            frictionAcceleration * dt
        );

        body.velocity +=
            tangent * (newSpeed - tangentSpeed);
    }
}
