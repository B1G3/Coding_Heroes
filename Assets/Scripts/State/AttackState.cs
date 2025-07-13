using UnityEngine;

public class AttackState : IUnitState
{
    private readonly IAttackable _target;
    private readonly int _maxHits;
    private int _hitCount;

    /// <param name="maxHits">이 상태 동안 허용할 최대 공격 횟수 (기본 1)</param>
    public AttackState(IAttackable target = null, int maxHits = 1)
    {
        _target = target ?? new DataContainer();
        _maxHits = maxHits;
    }

    public void Enter(Unit unit)
    {
        // 공격 활성화
        unit.canAttack = true;
        _hitCount = 0;
        // 충돌 이벤트 구독
        unit.OnAttackHit += OnHit;
        // 공격 애니메이션 트리거
    }

    private void OnHit(GameObject target)
    {
        if(_target is DataContainer)
        {
            _hitCount++;
            target.GetComponent<DataContainer>()?.OnAttack();
        }
        // 데미지 처리, 이펙트 등
    }

    public void Update(Unit unit)
    {
        // (필요시) 공격 중 행동 제어
    }

    public bool IsCompleted(Unit unit)
    {
        // 지정된 횟수만큼 공격이 발생했으면 완료
        return _hitCount >= _maxHits;
    }

    public void Exit(Unit unit)
    {
        // 공격 비활성화
        unit.canAttack = false;
        // 이벤트 해제
        unit.OnAttackHit -= OnHit;
        // 공격 종료 애니메이션 or 쿨다운 시작
    }
}