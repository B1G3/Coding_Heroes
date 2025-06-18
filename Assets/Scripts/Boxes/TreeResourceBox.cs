public class TreeResourceBox : BoxBase, IHarvestable
{
    public bool TryHarvest(out int resourceAmount) {
        resourceAmount = 1;
        return true;
    }
}
