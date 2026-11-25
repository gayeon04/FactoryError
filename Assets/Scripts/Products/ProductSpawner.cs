
using System;
using UnityEngine;

public class ProductSpawner : MonoBehaviour
{
    [Header("References")]
    public PhysicsWorld physicsWorld;
    public Product productPrefab;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public ProductData[] productSequence;

    [Min(0.01f)]
    public float spawnInterval = 1f;

    public int maxSpawnCount = 30;
    public bool randomSpawn = false;

    [Header("Runtime")]
    public bool isSpawning = true;
    public int spawnedCount;

    private float timer;

    private void FixedUpdate()
    {
        if (!isSpawning)
            return;

        if (spawnedCount >= maxSpawnCount)
            return;

        timer += Time.fixedDeltaTime;

        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            SpawnProduct();
        }
    }

    private void SpawnProduct()
    {
        if (productSequence == null ||
            productSequence.Length == 0 ||
            productPrefab == null ||
            physicsWorld == null ||
            spawnPoint == null)
            return;

        int index;

        if (randomSpawn)
        {
            index = UnityEngine.Random.Range(0, productSequence.Length);
        }
        else
        {
            index = spawnedCount %
                    productSequence.Length;
        }

        ProductData data = productSequence[index];

        if (data == null)
            return;

        Product product = Instantiate(
            productPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        product.Initialize(data, physicsWorld);

        spawnedCount++;
    }

    public void ResetSpawner()
    {
        spawnedCount = 0;
        timer = 0f;
    }
}
