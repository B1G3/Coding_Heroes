using static BlockConfig;

public interface IOutput
{
    bool CanSend(Direction dir);
    bool TrySend(out int amount, Direction dir);
    
    void SetOutputDirection(Direction dir);
    Direction GetOutputDirection();
    // void RegisterOutputTarget(IInput target, Direction toDir);
}