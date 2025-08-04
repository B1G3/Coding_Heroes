public class IdleState : IUnitState
{
    private bool _isCompleted = false;
    

    public void Enter(Unit unit)
    {
         /* 대기 애니메이션 */
         _isCompleted = true;
    }
    public void Update(Unit unit)  { /* 그냥 대기 */ }
    public bool IsCompleted(Unit unit)
    {
        return _isCompleted;
    }
    
    public void Exit(Unit unit)    { /* 아무것도 안 함 */ }
}