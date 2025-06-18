
public class TreeHarvester : HarvestModule
{
    private IHarvestable _target;

    public void InitializeTarget(IHarvestable target)
    {
        _target = target;
    }

    protected override bool PerformHarvest(out int amount)
    {
        if (_target != null && _target.TryHarvest(out amount))
            return true;

        amount = 0;
        return false;
    }
}