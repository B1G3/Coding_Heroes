using UnityEngine;

public class EnemyPath : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject StartDoorObject;
    [SerializeField] private GameObject EndObject;
    [SerializeField] private GameObject dataContainerPrefab;
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
        SpawnDataContainer(StartPos, EndPos);
    }

    private void SpawnDataContainer(Vector3 start, Vector3 end)
    {
        // 1) 인스턴스 생성
        var worm = Instantiate(dataContainerPrefab, start, Quaternion.identity);
        
        // 2) WormMover 붙이고 초기화
        var mover = worm.AddComponent<DataContainerMover>();
        mover.Initialize(start, end);
        mover.OnMoveComplete += OnReset;
    }
    
    private void OnReset()
    {
        doorAnimator.SetBool(IsOpenHash, false);
    }
}
