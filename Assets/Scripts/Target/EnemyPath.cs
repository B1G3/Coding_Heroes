using UnityEngine;

public class EnemyPath : MonoBehaviour, ITarget
{
    public Vector3 Position { get; set; }

    private void Start()
    {
        Position = transform.position;
    }
}
