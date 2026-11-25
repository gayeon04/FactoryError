
using UnityEngine;

[RequireComponent(typeof(PhysicsBox))]
public class Conveyor : MonoBehaviour
{
    public float beltSpeed = 3f;

    [Min(0f)]
    public float maxAcceleration = 10f;

    private PhysicsBox physicsBox;

    private void Awake()
    {
        physicsBox = GetComponent<PhysicsBox>();
    }

    public void ApplyConveyor(
        PhysicsBody body,
        CollisionInfo collision,
        float dt)
    {
        // 컨베이어 윗면에 접촉했는지 확인
        float alignment = Vector2.Dot(
            collision.normal,
            physicsBox.AxisY
        );

        if (alignment < 0.95f)
            return;

        // 컨베이어의 이동 방향
        Vector2 tangent = physicsBox.AxisX;

        // 현재 접선 방향 속도
        float currentSpeed = Vector2.Dot(
            body.velocity, tangent
        );

        // 목표 속도로 점진적으로 변경
        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            beltSpeed,
            maxAcceleration * dt
        );

        float deltaSpeed = newSpeed - currentSpeed;

        body.velocity += tangent * deltaSpeed;
    }
}
