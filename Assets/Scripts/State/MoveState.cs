using System.Collections;
using UnityEngine;

public class MoveState : IUnitState
{
    private ITarget _target;
    private readonly float _heightOffset;
    
    public MoveState(ITarget target, float offset)
    {
        _target = target;
        _heightOffset = offset;
    }
    
    // 타겟 업데이트 메서드 추가
    public void SetTarget(ITarget newTarget)
    {
        _target = newTarget;
    }
    
    private Vector3 goal;

    public void Enter(Unit unit)
    {
        if (_target != null)
        {
            goal = _target.Position;
            goal.y += _heightOffset;
        }
        // 이동 애니메이션 시작
    }

    public void Update(Unit unit)
    {
        if (_target == null) return;
        
        // 방향 바라보기 (부드럽게)
        Vector3 dir = goal - unit.transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion toRot = Quaternion.LookRotation(dir.normalized);
            float rotSpeed = unit.RotationSpeed * Time.deltaTime;
            unit.transform.rotation = Quaternion.Slerp(
                unit.transform.rotation,
                toRot,
                rotSpeed
            );
        }

        // 이동
        unit.transform.position = Vector3.MoveTowards(
            unit.transform.position,
            goal,
            unit.Speed * Time.deltaTime
        );
    }

    public bool IsCompleted(Unit unit)
    {
        if (_target == null) return true;
        return Vector3.Distance(unit.transform.position, goal) < 0.01f;
    }

    public void Exit(Unit unit)
    {
        // 도착 애니메이션 or 이벤트 트리거
    }
}