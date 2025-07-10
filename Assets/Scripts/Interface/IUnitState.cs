public interface IUnitState
{
    /// <summary>상태 진입 시 1회 호출</summary>
    void Enter(Unit unit);

    /// <summary>매 프레임 호출. 상태별 행동 수행</summary>
    void Update(Unit unit);

    /// <summary>상태가 완료되었는지 검사</summary>
    bool IsCompleted(Unit unit);

    /// <summary>상태 종료(전환 직전) 시 1회 호출</summary>
    void Exit(Unit unit);
}