
using UnityEngine;

public class PlacedDevice : MonoBehaviour
{
    public DeviceData Data { get; private set; }

    private PhysicsWorld physicsWorld;
    private PhysicsBox physicsBox;
    private Magnet magnet;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public void Initialize(
        DeviceData data,
        PhysicsWorld world)
    {
        Data = data;
        physicsWorld = world;

        physicsBox = GetComponent<PhysicsBox>();
        magnet = GetComponent<Magnet>();

        if (physicsBox != null)
            physicsWorld.RegisterBox(physicsBox);

        if (magnet != null)
            physicsWorld.RegisterMagnet(magnet);
    }

    public void RemoveDevice()
    {
        Unregister();

        gameObject.SetActive(false);

        Destroy(gameObject);
    }

    private void Unregister()
    {
        if (physicsWorld == null)
            return;

        if (physicsBox != null)
            physicsWorld.UnregisterBox(physicsBox);

        if (magnet != null)
            physicsWorld.UnregisterMagnet(magnet);
    }

    private void OnDestroy()
    {
        Unregister();
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void SetSelected(bool value)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = value
            ? Color.Lerp(originalColor, Color.yellow, 0.6f)
            : originalColor;
    }
}
