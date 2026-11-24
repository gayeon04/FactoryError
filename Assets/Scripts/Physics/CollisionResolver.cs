
using UnityEngine;

public static class CollisionResolver
{
    public static void Resolve(
        PhysicsBody body,
        PhysicsBox box,
        CollisionInfo collision)
    {
        if (!collision.isColliding)
            return;

        Vector2 normal = collision.normal;

        // 1. 침투한 만큼 위치 보정
        body.transform.position +=
            (Vector3)(
                normal * collision.penetration
            );

        // 2. 법선 방향 속도
        float velocityAlongNormal =
            Vector2.Dot(body.velocity, normal);

        // 이미 표면에서 멀어지는 경우
        if (velocityAlongNormal >= 0f)
            return;

        // 3. 반발계수 계산
        float e = Mathf.Clamp01(
            body.restitution * box.restitution
        );

        // 작은 충돌에서는 반발 억제
        if (Mathf.Abs(velocityAlongNormal) < 0.5f)
            e = 0f;

        // 4. 충돌 후 속도 계산
        body.velocity -=
            (1f + e)
            * velocityAlongNormal
            * normal;
    }
}
