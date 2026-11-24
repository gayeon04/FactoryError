
using System.Collections.Generic;
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

    private void Start()
    {
        bodies.AddRange(
            FindObjectsByType<PhysicsBody>(
                FindObjectsSortMode.None
            )
        );
        boxes.AddRange(
            FindObjectsByType<PhysicsBox>(
                FindObjectsSortMode.None
            )
        );
    }

    public void RegisterBody(PhysicsBody body)
    {
        if (!bodies.Contains(body))
            bodies.Add(body);
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
        }
    }

    private void Simulate(float dt)
    {
        foreach (PhysicsBody body in bodies)
        {
            if (body == null)
                continue;

            // 물리 운동
            body.AddForce(gravity * body.mass);
            body.Integrate(dt);

            // 충돌 검사
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

                CollisionResolver.Resolve(
                    body, box, collision
                );
            }
        }
    }
}
