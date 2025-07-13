using UnityEngine;

public class EnemyPath : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject StartDoorObject;
    [SerializeField] private GameObject EndObject;
    public Vector3 Position { get; set; }
    public Vector3 StartPos { get; set; }
    public Vector3 EndPos { get; set; }
    
    private Animator doorAnimator;
    private static readonly int IsOpenHash = Animator.StringToHash("isOpen");
    
    private void Start()
    {
        Position = target.transform.position;
        StartPos = StartDoorObject.transform.position;
        EndPos = EndObject.transform.position;
        doorAnimator = StartDoorObject.GetComponent<Animator>();
    }
    
    public void StartGame()
    {
        doorAnimator.SetBool(IsOpenHash, true);
    }
}
