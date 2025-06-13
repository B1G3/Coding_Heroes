public interface IHarvestable {
    bool TryHarvest(out int resourceAmount);
}
