
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PhysicsWorld : MonoBehaviour
{
    public Vector2 gravity = new Vector2(0f, -9.81f);


    [Range(1, 8)]
    public int substeps = 4;

    private List<PhysicsBody> bodies =
    new List<PhysicsBody>();

    private List<PhysicsBox> boxes =
    new List<PhysicsBox>();

    private ProductCollector[] collectors;

    private void Start()
    {
        foreach (PhysicsBody body in
            FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None))
        {
            RegisterBody(body);
        }

        foreach (PhysicsBox box in
            FindObjectsByType<PhysicsBox>(FindObjectsSortMode.None))
        {
            RegisterBox(box);
        }

        collectors = FindObjectsByType<ProductCollector>(
            FindObjectsSortMode.None
        );
    }

    public void RegisterBody(PhysicsBody body)
    {
        if (!bodies.Contains(body))
            bodies.Add(body);

        UnityEngine.Debug.Log(
                $"Register: {body.GetInstanceID()}, " +
                $"Total: {bodies.Count}"
            );
    }

    public void UnregisterBody(PhysicsBody body)
    {
        bodies.Remove(body);
    }

    public void RegisterBox(PhysicsBox box)
    {
        if (!boxes.Contains(box))
            boxes.Add(box);
    }

    public void UnregisterBox(PhysicsBox box)
    {
        boxes.Remove(box);
    }


    private void FixedUpdate()
    {
        int steps = Mathf.Max(1, substeps);
        float dt = Time.fixedDeltaTime / steps;

        for (int i = 0; i < steps; i++)
        {
            Simulate(dt);

            CheckCollectors();
        }
    }

    private void CheckCollectors()
    {
        for (int i = bodies.Count - 1; i >= 0; i--)
        {
            PhysicsBody body = bodies[i];

            if (body == null)
                continue;

            if (!body.TryGetComponent<Product>(
                out Product product))
                continue;

            foreach (ProductCollector collector
                     in collectors)
            {
                if (collector == null)
                    continue;

                collector.CheckProduct(product);

                if (product.IsCollected)
                    break;
            }
        }
    }





    private void Simulate(float dt)
    {
        foreach (PhysicsBody body in bodies)
        {
            if (body == null)
                continue;

            body.AddForce(gravity * body.mass);
            body.Integrate(dt);

            foreach (PhysicsBox box in boxes)
            {
                if (box == null)
                    continue;

                CollisionInfo collision =
                    CollisionDetector.CircleOBB(
                        body, box
                    );

                if (!collision.isColliding)
                    continue;

                // 기존 충돌 반응
                CollisionResolver.Resolve(
                    body, box, collision
                );

                // 신규: 컨베이어 접촉 처리
                if (box.TryGetComponent<Conveyor>(
                    out Conveyor conveyor))
                {
                    conveyor.ApplyConveyor(
                        body, collision, dt
                    );
                }
            }
        }


    }

}
