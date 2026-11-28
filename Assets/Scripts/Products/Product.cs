
using UnityEngine;

[RequireComponent(typeof(PhysicsBody))]
[RequireComponent(typeof(SpriteRenderer))]
public class Product : MonoBehaviour
{
    public ProductData Data { get; private set; }
    public PhysicsBody Body { get; private set; }

    public bool IsCollected { get; private set; }
    public bool IsBroken { get; private set; }

    private SpriteRenderer spriteRenderer;
    private PhysicsWorld physicsWorld;

    private void Awake()
    {
        Body = GetComponent<PhysicsBody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(
        ProductData data,
        PhysicsWorld world)
    {
        Data = data;
        physicsWorld = world;

        Body.mass = data.mass;
        Body.radius = data.radius;
        Body.restitution = data.restitution;

        spriteRenderer.color = data.color;

        // 기본 Circle Sprite의 지름이 1이라는 전제
        float diameter = data.radius * 2f;

        transform.localScale =
            new Vector3(diameter, diameter, 1f);

        IsCollected = false;
        IsBroken = false;

        Body.velocity = Vector2.zero;
        Body.acceleration = Vector2.zero;

        physicsWorld.RegisterBody(Body);
    }

    public bool TryCollect()
    {
        if (IsCollected)
            return false;

        IsCollected = true;

        physicsWorld.UnregisterBody(Body);

        Destroy(gameObject);

        return true;
    }

    private void OnDestroy()
    {
        if (physicsWorld != null)
            physicsWorld.UnregisterBody(Body);
    }

    public void CheckDamage(float impactSpeed)
    {
        if (Data == null ||
            !Data.isFragile ||
            IsBroken)
            return;

        if (impactSpeed >= Data.breakThreshold)
        {
            IsBroken = true;

            spriteRenderer.color = Color.red;
        }
    }
}


