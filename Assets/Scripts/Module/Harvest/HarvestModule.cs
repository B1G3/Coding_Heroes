using UnityEngine;

public abstract class HarvestModule : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)]
    private float harvestInterval = 1f; // 초 단위 수확 간격
    protected float lastHarvestTime;
    
    public void SetHarvestInterval(float seconds)
    {
        harvestInterval = Mathf.Clamp(seconds, 0.1f, 10f);
        lastHarvestTime = Time.time;
    }

    public virtual bool CanHarvest()
    {
        return Time.time - lastHarvestTime >= harvestInterval;
    }

    public bool TryHarvest(out int amount)
    {
        if (!CanHarvest())
        {
            amount = 0;
            return false;
        }

        bool success = PerformHarvest(out amount);
        if (success)
            lastHarvestTime = Time.time;

        return success;
    }

    // 실질적인 수확 구현은 하위 클래스가 담당
    protected abstract bool PerformHarvest(out int amount);
}