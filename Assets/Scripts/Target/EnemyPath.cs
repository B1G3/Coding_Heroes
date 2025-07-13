using UnityEngine;

public class EnemyPath : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject StartObject;
    [SerializeField] private GameObject EndObject;
    public Vector3 Position { get; set; }
    public Vector3 StartPos { get; set; }
    public Vector3 EndPos { get; set; }

    private void Start()
    {
        Position = target.transform.position;
        StartPos = StartObject.transform.position;
        EndPos = EndObject.transform.position;
    }
}
