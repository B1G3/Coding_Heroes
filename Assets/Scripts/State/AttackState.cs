using System;
using UnityEngine;

public class AttackState : IUnitState
{
    private IAttackable _specificTarget; // 특정 타겟 (If에서 지정된 경우)
    private readonly int _maxHits;
    private int _hitCount;
    private readonly bool _attackAnyTarget; // 아무 타겟이나 공격할지 여부
    
    // 데이터 필터 함수 추가
    private Func<DataContainer, bool> _dataFilter;

    /// <param name="specificTarget">특정 타겟 (null이면 아무 적이나 공격)</param>
    /// <param name="maxHits">최대 공격 횟수</param>
    public AttackState(IAttackable specificTarget = null, int maxHits = 1)
    {
        _specificTarget = specificTarget;
        _maxHits = maxHits;
        _attackAnyTarget = (specificTarget == null); // 특정 타겟이 없으면 아무나 공격
    }

    public void Enter(Unit unit)
    {
        unit.canAttack = true;
        _hitCount = 0;
        unit.OnAttackHit += OnHit;
    }

    public void SetTarget(IAttackable target)
    {
        _specificTarget = target;
    }
    
    // 데이터 필터 설정
    public void SetDataFilter(Func<DataContainer, bool> filter)
    {
        _dataFilter = filter;
    }

    private void OnHit(GameObject target)
    {
        bool shouldAttack = false;

        if (_attackAnyTarget)
        {
            // 아무 적이나 공격 (하지만 필터가 있으면 필터 적용)
            shouldAttack = IsValidEnemy(target);
        }
        else if (_specificTarget != null)
        {
            // 특정 타겟만 공격
            shouldAttack = IsSpecificTarget(target);
        }

        if (shouldAttack)
        {
            _hitCount++;
            
            // 데미지 처리
            var dataContainer = target.GetComponent<DataContainer>();
            if (dataContainer != null)
            {
                dataContainer.OnAttack();
            }
        }
    }

    private bool IsValidEnemy(GameObject target)
    {
        // DataContainer 컴포넌트 확인
        var dataContainer = target.GetComponent<DataContainer>();
        if (dataContainer != null)
        {
            // 필터가 있으면 필터로 확인, 없으면 모든 DataContainer 허용
            return _dataFilter?.Invoke(dataContainer) ?? true;
        }
        
        // 기존 로직
        return target.CompareTag("Enemy") || 
               target.GetComponent<IAttackable>() != null;
    }

    private bool IsSpecificTarget(GameObject target)
    {
        // 특정 타겟과 일치하는지 확인
        if (_specificTarget is Component component)
        {
            return target == component.gameObject;
        }
        
        // ITarget 인터페이스를 통한 비교
        if (_specificTarget is ITarget targetInterface)
        {
            float distance = Vector3.Distance(target.transform.position, targetInterface.Position);
            return distance < 0.5f; // 임계값
        }
        
        return target.GetComponent<IAttackable>() == _specificTarget;
    }

    public void Update(Unit unit)
    {
        // 필요시 추가 로직
    }

    public bool IsCompleted(Unit unit)
    {
        return _hitCount >= _maxHits;
    }

    public void Exit(Unit unit)
    {
        unit.canAttack = false;
        unit.OnAttackHit -= OnHit;
    }
}