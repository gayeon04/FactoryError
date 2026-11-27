
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PhysicsWorld : MonoBehaviour
{
    public Vector2 gravity = new Vector2(0f, -9.81f);


    [Range(1, 8)]
    public int substeps = 4;

    public bool isSimulating = true;

    public IReadOnlyList<PhysicsBox> Boxes => boxes;

    private List<PhysicsBody> bodies =
    new List<PhysicsBody>();

    private List<PhysicsBox> boxes =
    new List<PhysicsBox>();

    private List<Magnet> magnets =
    new List<Magnet>();

    private ProductCollector[] collectors;

    [Header("Waste Settings")]
    public Vector2 wasteMin = new Vector2(-12f, -6f);
    public Vector2 wasteMax = new Vector2(12f, 12f);

    public int WastedCount { get; private set; }

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

        magnets.AddRange(
            FindObjectsByType<Magnet>(
                FindObjectsSortMode.None)
        );

        collectors = FindObjectsByType<ProductCollector>(
            FindObjectsSortMode.None
        );
    }

    #region Registration

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

    public void RegisterMagnet(Magnet magnet)
    {
        if (!magnets.Contains(magnet))
            magnets.Add(magnet);
    }

    public void UnregisterMagnet(Magnet magnet)
    {
        magnets.Remove(magnet);
    }
    #endregion

    private void FixedUpdate()
    {
        // 1. 힘 적용 2. Integrate (가속도 -> 속도 -> 위치 갱신)
        // 3. collision detection 4. collision response
        // 5. 접속 장치 처리 6. 제품 수거 검사

        if (!isSimulating)
            return;

        int steps = Mathf.Max(1, substeps);
        float dt = Time.fixedDeltaTime / steps;

        for (int i = 0; i < steps; i++)
        {
            Simulate(dt);

            CheckCollectors();

            CheckWaste();
        }
    }

    private void CheckCollectors()
    {
        for (int i = bodies.Count - 1; i >= 0; i--)
        {
            PhysicsBody body = bodies[i];

            if (body == null ||
               !body.isActiveAndEnabled)
                continue;

            if (!body.TryGetComponent<Product>(
                out Product product))
                continue;

            foreach (ProductCollector collector
                     in collectors)
            {
                if (collector == null ||
                    !collector.isActiveAndEnabled)
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
            if (body == null ||
            !body.isActiveAndEnabled)
                continue;

            // 1. 중력
            body.AddForce(gravity * body.mass);

            // 2. 전자석
            foreach (Magnet magnet in magnets)
            {
                if (magnet == null ||
                    !magnet.isActiveAndEnabled)
                    continue;

                magnet.ApplyMagnet(body);
            }

            // 3. 물리 운동
            body.Integrate(dt);

            // 4. 충돌 검사
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

                // 5. 충돌 반응
                CollisionResolver.Resolve(
                    body, box, collision
                );

                // 6. 접촉 장치 처리
                ApplyContactDevices(
                    body, box, collision, dt
                );
            }
        }
    }
        
    private void ApplyContactDevices(
        PhysicsBody body,
        PhysicsBox box,
        CollisionInfo collision,
        float dt)
    {
        // 컨베이어
        if (box.TryGetComponent<Conveyor>(
            out Conveyor conveyor)
            && conveyor.isActiveAndEnabled)
        {
            conveyor.ApplyConveyor(
                body, collision, dt
            );
        }

        // 발사기
        if (box.TryGetComponent<Launcher>(
            out Launcher launcher)
            && launcher.isActiveAndEnabled)
        {
            launcher.ApplyLauncher(
                body, collision
            );
        }

        // 감속판
        if (box.TryGetComponent<FrictionPlate>(
            out FrictionPlate frictionPlate)
            && frictionPlate.isActiveAndEnabled)
        {
            frictionPlate.ApplyFriction(
                body, collision, gravity, dt
            );
        }
    }

    private void CheckWaste()
    {
        for (int i = bodies.Count - 1; i >= 0; i--)
        {
            PhysicsBody body = bodies[i];

            if (body == null)
                continue;

            if (!body.TryGetComponent<Product>(
                out Product product))
                continue;

            if (product.IsCollected)
                continue;

            Vector2 position = body.transform.position;

            bool isOutOfBounds =
                position.x < wasteMin.x ||
                position.x > wasteMax.x ||
                position.y < wasteMin.y ||
                position.y > wasteMax.y;

            if (!isOutOfBounds)
                continue;

            if (product.TryCollect())
            {
                WastedCount++;
            }
        }
    }


    public void ResetSimulation()
    {
        isSimulating = false;

        // 생성된 제품 제거
        for (int i = bodies.Count - 1; i >= 0; i--)
        {
            PhysicsBody body = bodies[i];

            if (body == null)
            {
                bodies.RemoveAt(i);
                continue;
            }

            if (body.TryGetComponent<Product>(
                out Product product))
            {
                product.TryCollect();
            }
        }

        WastedCount = 0;

        // 발사기 내부 기록 초기화
        foreach (PhysicsBox box in boxes)
        {
            if (box == null)
                continue;

            if (box.TryGetComponent<Launcher>(
                out Launcher launcher))
            {
                launcher.ResetLauncher();
            }
        }
    }


}
