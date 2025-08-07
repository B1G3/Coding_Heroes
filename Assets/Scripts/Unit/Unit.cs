using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private IUnitState CurrentState;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private BoxCollider attackFilter;
    // [SerializeField] private TMP_Text text;
    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
    private bool _canAttack;
    public bool canAttack
    {
        get => _canAttack;
        set
        {
            _canAttack = value;
            attackFilter.enabled = value;
        }
    }

    private List<IUnitState> _command;
    private int _currentStateIndex = -1; // 현재 실행 중인 state 인덱스

    public event Action<GameObject> OnAttackHit;
    public event Action OnFinishCommand;
    
    private void Awake()
    {
        canAttack = false;
    }
    
    private void Update()
    {
        // 상태별 매 프레임 로직 처리
        CurrentState?.Update(this);
    }
    
    /// <summary>
    /// 상태 리스트를 순차적으로 실행하고, 각 상태의 IsCompleted가 true 될 때까지 대기합니다.
    /// </summary>
    public async UniTaskVoid StartUnitAsync(List<IUnitState> command)
    {
        _command = command;
        _currentStateIndex = -1;
        
        for (int i = 0; i < command.Count; i++)
        {
            _currentStateIndex = i; // 현재 실행 중인 state 인덱스 업데이트
            var state = command[i];
            
            // text.text = $"{state}";
            ChangeState(state);
            await UniTask.WaitUntil(() => state.IsCompleted(this));
            CurrentState?.Exit(this);
        }

        _currentStateIndex = -1; // 완료 후 초기화
        FinishCommandAsync().Forget();
    }

    // 현재 진행중인 state 뒤에 있는 state들만 반환
    public List<IUnitState> GetRemainingCommand()
    {
        if (_command == null || _currentStateIndex < 0) 
            return new List<IUnitState>();

        // 현재 state 다음부터 끝까지 반환
        return _command.Skip(_currentStateIndex + 1).ToList();
    }
    
    // 전체 command 반환 (기존 호환성)
    public List<IUnitState> GetCommand()
    {
        return _command ?? new List<IUnitState>();
    }
    
    // 현재 실행 중인 state 반환
    public IUnitState GetCurrentState()
    {
        if (_command == null || _currentStateIndex < 0 || _currentStateIndex >= _command.Count)
            return null;
            
        return _command[_currentStateIndex];
    }
    
    private void ChangeState(IUnitState state)
    {
        CurrentState = state;
        CurrentState?.Enter(this);
    }
    
    // attackFilter 가 isTrigger = true 여야 합니다.
    private void OnTriggerEnter(Collider other)
    {
        // text.text = $"{other.gameObject.name}";
        if (!canAttack) return;
        // 필터링(예: 적 태그)하고 싶으면 여기서 검사 가능
        OnAttackHit?.Invoke(other.gameObject);
    }

    private async UniTaskVoid FinishCommandAsync()
    {
        OnFinishCommand?.Invoke();
        await UniTask.Yield();
        Destroy(gameObject);
    }
}