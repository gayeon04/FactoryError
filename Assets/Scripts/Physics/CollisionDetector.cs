
using UnityEngine;

public static class CollisionDetector
{
    public static CollisionInfo CircleOBB(
        PhysicsBody circle,
        PhysicsBox box)
    {
        Vector2 center = circle.transform.position;
        Vector2 boxCenter = box.Center;

        Vector2 axisX = box.AxisX;
        Vector2 axisY = box.AxisY;

        Vector2 halfSize = box.HalfSize;

        // 1. 사각형 중심에서 원 중심까지
        Vector2 diff = center - boxCenter;

        // 2. OBB 로컬 좌표계로 투영
        float localX = Vector2.Dot(diff, axisX);
        float localY = Vector2.Dot(diff, axisY);

        // 3. 사각형 내부의 최근접점
        float closestX = Mathf.Clamp(
            localX, -halfSize.x, halfSize.x);

        float closestY = Mathf.Clamp(
            localY, -halfSize.y, halfSize.y);

        // 4. 월드 좌표계로 변환
        Vector2 closestPoint =
            boxCenter
            + axisX * closestX
            + axisY * closestY;

        Vector2 delta = center - closestPoint;

        float distanceSqr = delta.sqrMagnitude;
        float radius = circle.radius;

        // 5. 충돌하지 않은 경우
        if (distanceSqr > radius * radius)
            return new CollisionInfo();

        // 6. 원의 중심이 사각형 외부인 경우
        if (distanceSqr > 0.00000001f)
        {
            float distance = Mathf.Sqrt(distanceSqr);

            return new CollisionInfo
            {
                isColliding = true,
                normal = delta / distance,
                contactPoint = closestPoint,
                penetration = radius - distance
            };
        }

        // 7. 원의 중심이 사각형 내부인 경우
        float depthX =
            halfSize.x - Mathf.Abs(localX);

        float depthY =
            halfSize.y - Mathf.Abs(localY);

        Vector2 normal;
        float depth;

        if (depthX < depthY)
        {
            normal = axisX *
                (localX >= 0f ? 1f : -1f);

            depth = depthX;
        }
        else
        {
            normal = axisY *
                (localY >= 0f ? 1f : -1f);

            depth = depthY;
        }

        return new CollisionInfo
        {
            isColliding = true,
            normal = normal,
            contactPoint = center + normal * depth,
            penetration = radius + depth
        };
    }
}
