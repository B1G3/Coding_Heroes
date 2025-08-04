public interface IGameState
{
    void Enter();
    void Exit();
    bool CanPlaceBlock { get; }
    bool CanRotateBlock { get; }
    bool CanReposition { get; }
}