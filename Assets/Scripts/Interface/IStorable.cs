public interface IStorable {
    bool CanStore(int amount);
    bool TryStore(int amount);
}