using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private IUnitState CurrentState;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private BoxCollider attackFilter;
    [SerializeField] private TMP_Text text;
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

    public event Action<GameObject> OnAttackHit;
    
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
        foreach (var state in command)
        {
            text.text = $"{state}";
            ChangeState(state);
            await UniTask.WaitUntil(() => state.IsCompleted(this));
            CurrentState?.Exit(this);
        }
        
    }
    
    private void ChangeState(IUnitState state)
    {
        CurrentState = state;
        CurrentState?.Enter(this);
    }
    
    // attackFilter 가 isTrigger = true 여야 합니다.
    private void OnTriggerEnter(Collider other)
    {
        text.text = $"{other.gameObject.name}";
        if (!canAttack) return;
        // 필터링(예: 적 태그)하고 싶으면 여기서 검사 가능
        OnAttackHit?.Invoke(other.gameObject);
    }

    private void OnFinishCommand()
    {
        
    }
}
