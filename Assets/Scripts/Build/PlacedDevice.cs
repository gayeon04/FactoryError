
using UnityEngine;

public class PlacedDevice : MonoBehaviour
{
    public DeviceData Data { get; private set; }

    private PhysicsWorld physicsWorld;
    private PhysicsBox physicsBox;
    private Magnet magnet;

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
}
