using System.Collections;
using UnityEngine;

public class MoveState : IUnitState
{
    private readonly ITarget _target;
    private readonly float _heightOffset = 0.1f;
    
    public MoveState(ITarget target) => _target = target;
    private Vector3 goal;

    public void Enter(Unit unit)
    {
        goal = _target.Position;
        goal.y += _heightOffset;
        // 이동 애니메이션 시작
    }

    public void Update(Unit unit)
    {
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
        return Vector3.Distance(unit.transform.position, goal) < 0.05f;
    }

    public void Exit(Unit unit)
    {
        // 코루틴으로 카메라 바라보게 회전 시작
        var cam = Camera.main;
        if (cam != null)
        {
            // 카메라와 같은 높이에서 바라보도록 방향 계산
            Vector3 lookPos = cam.transform.position;
            lookPos.y = unit.transform.position.y;
            Vector3 direction = lookPos - unit.transform.position;

            // Unit은 MonoBehaviour이므로 StartCoroutine 호출 가능
            unit.StartCoroutine(RotateToCamera(unit.transform, direction, unit.RotationSpeed));
        }
        
        // 도착 애니메이션 or 이벤트 트리거
    }
    
    private IEnumerator RotateToCamera(Transform mover, Vector3 direction, float rotationSpeed)
    {
        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);

        // 목표 회전까지 남은 각도가 충분히 작아질 때까지 매 프레임 회전
        while (Quaternion.Angle(mover.rotation, targetRot) > 0.5f)
        {
            mover.rotation = Quaternion.RotateTowards(
                mover.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 끝까지 맞춰주기
        mover.rotation = targetRot;
    }
}