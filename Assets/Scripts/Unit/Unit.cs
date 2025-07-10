using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private IUnitState CurrentState;
    [SerializeField] private float speed = 3f;
    public float Speed => speed;
    
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
            ChangeState(state);
            await UniTask.WaitUntil(() => state.IsCompleted(this));
        }
    }
    
    private void ChangeState(IUnitState state)
    {
        CurrentState?.Exit(this);
        CurrentState = state;
        CurrentState?.Enter(this);
    }
}
