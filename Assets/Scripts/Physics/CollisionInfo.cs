using UnityEngine;

public struct CollisionInfo
{
    public bool isColliding;

    public Vector2 normal;
    public Vector2 contactPoint;

    public float penetration;
}