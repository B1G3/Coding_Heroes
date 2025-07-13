using UnityEngine;

public class DataContainer : MonoBehaviour, IAttackable
{
    public Vector3 Position { get; }
    
    public virtual void OnAttack()
    {
        Die();
    }

    protected virtual void Die()
    {
        // 공통 사망 로직 (이펙트, 사운드 등)
        Destroy(gameObject);
    }

    
}
