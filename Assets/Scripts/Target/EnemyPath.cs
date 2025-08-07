using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject dataContainerPrefab;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject startDoorObject;
    [SerializeField] private GameObject endObject;
    
    [SerializeField] private float spawnInterval = 2f;

    public Vector3 Position => target.transform.position;
    public Vector3 StartPos  => startDoorObject.transform.position;
    public Vector3 EndPos    => endObject.transform.position;
    
    private int remainingCount;

    private Animator doorAnimator;
    private static readonly int IsOpenHash = Animator.StringToHash("isOpen");

    // prefab별 오브젝트 풀
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
    
    // 컴포넌트 캐시 추가
    private readonly Dictionary<GameObject, DataContainerMover> moverCache = new();
    private readonly Dictionary<GameObject, DataContainer> containerCache = new();

    private void Awake()
    {
        doorAnimator = startDoorObject.GetComponent<Animator>();
    }

    public void StartGame(StageConfig stage)
    {
        doorAnimator.SetBool(IsOpenHash, true);
        
        List<GameObject> spawnList;
        if (!stage)
        {
            spawnList = new List<GameObject> { dataContainerPrefab };
        }
        else
        {
            spawnList = CreateSpawnList(stage);
            Shuffle(spawnList);
        }

        spawnList = CreateSpawnList(stage);
        Shuffle(spawnList);
        
        remainingCount = spawnList.Count;
        StartCoroutine(SpawnSequence(spawnList));
    }
    
    private IEnumerator SpawnSequence(List<GameObject> spawnList)
    {
        foreach (var prefab in spawnList)
        {
            Spawn(prefab);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn(GameObject prefab)
    {
        var instance = GetFromPool(prefab);
        instance.transform.position = StartPos;
        instance.SetActive(true);

        // 캐시에서 컴포넌트 가져오기
        if (!moverCache.TryGetValue(instance, out var mover))
        {
            mover = instance.GetComponent<DataContainerMover>() ?? instance.AddComponent<DataContainerMover>();
            moverCache[instance] = mover;
        }
        
        if (!containerCache.TryGetValue(instance, out var dataContainer))
        {
            dataContainer = instance.GetComponent<DataContainer>();
            containerCache[instance] = dataContainer;
        }
        
        dataContainer.SetMover(mover);
        
        // 재사용 전 정리
        mover.CancelMovement();
        mover.OnMoveComplete -= OnContainerArrived;
        
        // 새 이동 초기화
        mover.Initialize(prefab, StartPos, EndPos);
        mover.OnMoveComplete += OnContainerArrived;
    }

    private void OnContainerArrived(GameObject instance)
    {
        ReturnToPool(instance);
        remainingCount--;

        if (remainingCount <= 0)
        {
            doorAnimator.SetBool(IsOpenHash, false);
        }
    }

    private List<GameObject> CreateSpawnList(StageConfig stage)
    {
        var list = new List<GameObject>();

        foreach (var entry in stage.dummyEntries)
            for (int i = 0; i < entry.count; i++)
                list.Add(entry.prefab);

        foreach (var entry in stage.targetEntries)
            for (int i = 0; i < entry.count; i++)
                list.Add(entry.prefab);

        return list;
    }

    private void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var q))
            pools[prefab] = q = new Queue<GameObject>();

        return q.Count > 0 ? q.Dequeue() : Instantiate(prefab);
    }

    private void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);

        // 캐시에서 가져오기
        if (moverCache.TryGetValue(instance, out var mover))
        {
            var prefab = mover.Prefab;
            if (!pools.TryGetValue(prefab, out var q))
                pools[prefab] = q = new Queue<GameObject>();
            q.Enqueue(instance);
        }
    }
}