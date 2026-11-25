
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PhysicsBox))]
public class Launcher : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchAngle = 45f;
    public float launchSpeed = 10f;

    private PhysicsBox physicsBox;

    // 동일 제품의 중복 발사 방지
    private Dictionary<int, float> lastLaunchTimes =
        new Dictionary<int, float>();

    public float cooldown = 0.5f;

    private void Awake()
    {
        physicsBox = GetComponent<PhysicsBox>();
    }

    public void ApplyLauncher(
        PhysicsBody body,
        CollisionInfo collision)
    {
        // 윗면 접촉 여부 검사
        float alignment = Vector2.Dot(
            collision.normal,
            physicsBox.AxisY
        );

        if (alignment < 0.95f)
            return;

        int id = body.GetInstanceID();

        if (lastLaunchTimes.TryGetValue(
            id, out float lastTime))
        {
            if (Time.fixedTime - lastTime < cooldown)
                return;
        }

        float rad = launchAngle * Mathf.Deg2Rad;

        Vector2 direction =
            physicsBox.AxisX * Mathf.Cos(rad)
            + physicsBox.AxisY * Mathf.Sin(rad);

        body.velocity =
            direction.normalized * launchSpeed;

        lastLaunchTimes[id] = Time.fixedTime;
    }

    public void ResetLauncher()
    {
        lastLaunchTimes.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        float rad = launchAngle * Mathf.Deg2Rad;

        Vector3 direction =
            transform.right * Mathf.Cos(rad)
            + transform.up * Mathf.Sin(rad);

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            direction * 2f
        );
    }
}
