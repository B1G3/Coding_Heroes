using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject StartDoorObject;
    [SerializeField] private GameObject EndObject;
    [SerializeField] private GameObject dataContainerPrefab;

    public Vector3 Position { get; private set; }
    public Vector3 StartPos { get; private set; }
    public Vector3 EndPos   { get; private set; }

    // prefab 별 풀 관리
    private Dictionary<GameObject, Queue<GameObject>> _pools = new();

    private Animator doorAnimator;
    private static readonly int IsOpenHash = Animator.StringToHash("isOpen");

    private void Awake()
    {
        // Start 대신 Awake로 초기화하여 미리 준비
        Position = target.transform.position;
        StartPos = StartDoorObject.transform.position;
        EndPos   = EndObject.transform.position;
        doorAnimator = StartDoorObject.GetComponent<Animator>();
    }

    /// <summary>
    /// 게임 시작 시 호출
    /// </summary>
    public void StartGame(StageConfig stage)
    {
        if (stage == null) return;

        doorAnimator.SetBool(IsOpenHash, true);
        SpawnDataContainer(StartPos, EndPos);
    }

    private void SpawnDataContainer(Vector3 start, Vector3 end)
    {
        // 풀에서 꺼내거나 새로 생성
        var worm = GetFromPool(dataContainerPrefab);
        worm.transform.position = start;
        worm.SetActive(true);

        // 이동 컴포넌트 초기화
        var mover = worm.GetComponent<DataContainerMover>() ?? worm.AddComponent<DataContainerMover>();
        mover.Initialize(worm, start, end);

        // DataContainer에 mover 주입
        if (worm.TryGetComponent<DataContainer>(out var container))
            container.SetMover(mover);

        // 이동 완료 시 문 닫기 및 풀 복귀
        mover.OnMoveComplete -= OnContainerArrived;
        mover.OnMoveComplete += OnContainerArrived;
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        if (!_pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            _pools[prefab] = queue;
        }
        if (queue.Count > 0)
        {
            return queue.Dequeue();
        }
        else
        {
            // 새 인스턴스 생성
            var go = Instantiate(prefab);
            return go;
        }
    }

    private void OnContainerArrived(GameObject worm)
    {
        // 문 닫기
        doorAnimator.SetBool(IsOpenHash, false);

        // 풀에 반환
        ReturnToPool(worm);
    }

    private void ReturnToPool(GameObject worm)
    {
        worm.SetActive(false);

        // 원본 prefab 식별을 위해 DataContainerMover의 Prefab 프로퍼티 사용
        var mover = worm.GetComponent<DataContainerMover>();
        var prefab = mover != null ? mover.Prefab : dataContainerPrefab;

        if (!_pools.TryGetValue(prefab, out var queue))
            _pools[prefab] = queue = new Queue<GameObject>();

        queue.Enqueue(worm);
    }
}
