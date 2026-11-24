
using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    [Header("Physical Properties")]
    [Min(0.001f)] public float mass = 1f;
    public float radius = 0.25f;

    [Range(0f, 1f)]
    public float restitution = 0.5f;

    [Header("Runtime Data")]
    public Vector2 velocity;
    public Vector2 acceleration;

    private Vector2 force;

    public void AddForce(Vector2 newForce)
    {
        force += newForce;
    }

    public void AddImpulse(Vector2 impulse)
    {
        velocity += impulse / mass;
    }

    public void Integrate(float dt)
    {
        acceleration = force / mass;

        velocity += acceleration * dt;

        transform.position +=
            (Vector3)(velocity * dt);

        force = Vector2.zero;
    }
}
