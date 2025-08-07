using UnityEngine;

public class AttackState : IUnitState
{
    private readonly IAttackable _specificTarget; // 특정 타겟 (If에서 지정된 경우)
    private readonly int _maxHits;
    private int _hitCount;
    private readonly bool _attackAnyTarget; // 아무 타겟이나 공격할지 여부

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

    private void OnHit(GameObject target)
    {
        bool shouldAttack = false;

        if (_attackAnyTarget)
        {
            // 아무 적이나 공격 (태그나 컴포넌트로 적인지 확인)
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
        // 적으로 간주할 수 있는 오브젝트인지 확인
        return target.CompareTag("Enemy") || 
               target.GetComponent<DataContainer>() != null ||
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