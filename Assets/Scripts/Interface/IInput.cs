using static BlockConfig;

public interface IInput
{
    bool CanReceive(Direction dir);
    bool TryReceive(int amount, Direction dir);

    void SetInputDirection(Direction dir);
    Direction GetInputDirection();
}