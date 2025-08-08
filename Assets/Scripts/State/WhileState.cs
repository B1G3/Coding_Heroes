using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WhileState : IUnitState
{
    private readonly int _maxIterations;
    private int _currentIteration = 0;
    private bool _isCompleted = false;
    
    // 데이터 필터
    private Func<DataContainer, bool> _dataFilter;
    
    // 원래 자리와 현재 타겟
    private Vector3 _originalPosition;
    private DataContainer _currentTarget;
    
    // 🚨 이미 공격한 타겟들을 기록
    private HashSet<DataContainer> _attackedTargets = new HashSet<DataContainer>();
    
    // 내부 상태 관리
    private enum WhilePhase { WaitingForTarget, MovingToTarget, Attacking, Returning }
    private WhilePhase _currentPhase = WhilePhase.WaitingForTarget;

    public WhileState(int maxIterations = 2)
    {
        _maxIterations = maxIterations;
    }
    
    public void SetDataFilter(Func<DataContainer, bool> filter)
    {
        _dataFilter = filter;
        Debug.Log($"WhileState: 필터 설정됨 - {filter != null}");
    }

    public void Enter(Unit unit)
    {
        _isCompleted = false;
        _currentIteration = 0;
        _originalPosition = unit.transform.position;
        _currentPhase = WhilePhase.WaitingForTarget;
        _currentTarget = null;
        _attackedTargets.Clear(); // 공격 기록 초기화
        
        Debug.Log($"WhileState: Enter - 최대 반복 {_maxIterations}회");
    }

    public void Update(Unit unit)
    {
        switch (_currentPhase)
        {
            case WhilePhase.WaitingForTarget:
                UpdateWaitingForTarget(unit);
                break;
                
            case WhilePhase.MovingToTarget:
                UpdateMovingToTarget(unit);
                break;
                
            case WhilePhase.Attacking:
                UpdateAttacking(unit);
                break;
                
            case WhilePhase.Returning:
                UpdateReturning(unit);
                break;
        }
    }
    
    private void UpdateWaitingForTarget(Unit unit)
    {
        // 조건에 맞는 DataContainer 찾기
        Collider[] containers = Physics.OverlapSphere(unit.transform.position, 0.1f);
        
        foreach (var container in containers)
        {
            if (container.TryGetComponent<BadDataContainer>(out var dataContainer))
            {
                // 🚨 핵심: 이미 공격한 타겟이면 건너뛰기
                if (_attackedTargets.Contains(dataContainer))
                {
                    Debug.Log($"WhileState: {dataContainer.name} 이미 공격한 타겟, 건너뛰기");
                    continue;
                }
                
                // 🚨 핵심: 이미 죽었거나 비활성화된 타겟 건너뛰기
                if (!dataContainer.gameObject.activeInHierarchy || dataContainer.IsDead())
                {
                    Debug.Log($"WhileState: {dataContainer.name} 죽었거나 비활성화, 건너뛰기");
                    continue;
                }
                
                // 필터 조건 확인
                if (_dataFilter != null)
                {
                    bool isValidTarget = _dataFilter.Invoke(dataContainer);
                    Debug.Log($"WhileState: DataContainer {dataContainer.name} 필터 결과: {isValidTarget}");
                    
                    if (isValidTarget)
                    {
                        Debug.Log($"WhileState: {dataContainer.name} 새로운 타겟으로 설정!");
                        dataContainer.OnStop();
                        _currentTarget = dataContainer;
                        _currentPhase = WhilePhase.MovingToTarget;
                        return;
                    }
                }
                else
                {
                    // 필터가 없으면 모든 DataContainer 허용
                    Debug.Log($"WhileState: {dataContainer.name} 새로운 타겟으로 설정!");
                    dataContainer.OnStop();
                    _currentTarget = dataContainer;
                    _currentPhase = WhilePhase.MovingToTarget;
                    return;
                }
            }
        }
        
        // 타겟이 없고 반복 횟수 초과하면 완료
        if (_currentIteration >= _maxIterations)
        {
            _isCompleted = true;
            Debug.Log("WhileState: 반복 완료 - 더 이상 타겟 없음");
        }
    }
    
    private void UpdateMovingToTarget(Unit unit)
    {
        // 🚨 이동 중에 타겟이 죽었는지 확인
        if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy || _currentTarget.IsDead())
        {
            Debug.Log("WhileState: 이동 중 타겟이 사라짐, 새 타겟 찾기");
            _currentTarget = null;
            _currentPhase = WhilePhase.WaitingForTarget;
            return;
        }
        
        Vector3 targetPos = _currentTarget.transform.position;
        
        // 방향 바라보기
        Vector3 dir = targetPos - unit.transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion toRot = Quaternion.LookRotation(dir.normalized);
            float rotSpeed = unit.RotationSpeed * Time.deltaTime;
            unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, toRot, rotSpeed);
        }
        
        // 이동
        unit.transform.position = Vector3.MoveTowards(
            unit.transform.position,
            targetPos,
            unit.Speed * Time.deltaTime
        );
        
        // 도착했으면 공격 단계로
        if (Vector3.Distance(unit.transform.position, targetPos) < 0.01f)
        {
            _currentPhase = WhilePhase.Attacking;
            unit.canAttack = true;
        }
    }
    
    private void UpdateAttacking(Unit unit)
    {
        if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
        {
            _currentPhase = WhilePhase.Returning;
            unit.canAttack = false;
            return;
        }
        
        // 공격 로직
        _currentTarget.OnAttack();
        
        // 🚨 핵심: 공격한 타겟을 기록에 추가
        _attackedTargets.Add(_currentTarget);
        Debug.Log($"WhileState: {_currentTarget.name} 공격 완료 및 기록 추가");
        
        _currentTarget = null; // 공격 완료
        _currentPhase = WhilePhase.Returning;
        unit.canAttack = false;
        
        Debug.Log($"WhileState: 공격 완료, 반복 {_currentIteration + 1}/{_maxIterations}");
    }
    
    private void UpdateReturning(Unit unit)
    {
        // 원래 자리로 돌아가기
        Vector3 dir = _originalPosition - unit.transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion toRot = Quaternion.LookRotation(dir.normalized);
            float rotSpeed = unit.RotationSpeed * Time.deltaTime;
            unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, toRot, rotSpeed);
        }
        
        unit.transform.position = Vector3.MoveTowards(
            unit.transform.position,
            _originalPosition,
            unit.Speed * Time.deltaTime
        );
        
        // 원래 자리 도착
        if (Vector3.Distance(unit.transform.position, _originalPosition) < 0.01f)
        {
            _currentIteration++;
            
            if (_currentIteration >= _maxIterations)
            {
                _isCompleted = true;
                Debug.Log("WhileState: 모든 반복 완료!");
            }
            else
            {
                _currentPhase = WhilePhase.WaitingForTarget;
                Debug.Log($"WhileState: 다음 반복 시작 ({_currentIteration}/{_maxIterations})");
            }
        }
    }

    public bool IsCompleted(Unit unit)
    {
        return _isCompleted;
    }

    public void Exit(Unit unit)
    {
        unit.canAttack = false;
        _attackedTargets.Clear(); // 정리
        Debug.Log("WhileState: Exit");
    }
}