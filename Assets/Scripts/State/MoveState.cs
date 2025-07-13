using UnityEngine;

public class MoveState : IUnitState
{
    private readonly ITarget _target;
    public MoveState(ITarget target) => _target = target;

    public void Enter(Unit unit)
    {
        // 이동 애니메이션 시작
    }

    public void Update(Unit unit)
    {
        Vector3 dir = _target.Position - unit.transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            float t = unit.RotationSpeed * Time.deltaTime; // 0~1 사이 합리적 범위로 조정
            unit.transform.rotation = Quaternion.Slerp(
                unit.transform.rotation,
                targetRot,
                t
            );
        }

        unit.transform.position = Vector3.MoveTowards(
            unit.transform.position,
            _target.Position,
            unit.Speed * Time.deltaTime
        );
    }

    public bool IsCompleted(Unit unit)
    {
        // 목표 지점 도착 여부
        return Vector3.Distance(unit.transform.position, _target.Position) < 0.1f;
    }

    public void Exit(Unit unit)
    {
        // 도착 애니메이션 혹은 이벤트 트리거
    }
}