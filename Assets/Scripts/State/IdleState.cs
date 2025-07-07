public class IdleState : IUnitState
{
    public void Enter(Unit unit)   { /* 대기 애니메이션 */ }
    public void Update(Unit unit)  { /* 그냥 대기 */ }
    public bool IsCompleted(Unit unit) => false; // 절대 끝나지 않음
    public void Exit(Unit unit)    { /* 아무것도 안 함 */ }
}