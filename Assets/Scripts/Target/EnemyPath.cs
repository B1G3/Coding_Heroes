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
    
    [SerializeField] private float spawnInterval = 0.5f;

    public Vector3 Position => target.transform.position;
    public Vector3 StartPos  => startDoorObject.transform.position;
    public Vector3 EndPos    => endObject.transform.position;
    
    private int remainingCount;

    private Animator doorAnimator;
    private static readonly int IsOpenHash = Animator.StringToHash("isOpen");

    // prefab별 오브젝트 풀
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        doorAnimator = startDoorObject.GetComponent<Animator>();
    }

    /// <summary>
    /// 게임 시작 시 호출
    /// </summary>
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

        // 1) 전달된 stage 정보로 Flat List 생성·셔플
        spawnList = CreateSpawnList(stage);
        Shuffle(spawnList);
        
        // 남은 도착 횟수 초기화
        remainingCount = spawnList.Count;

        // 2) 섞인 순서대로 스폰
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

        // DataContainerMover 초기화
        var mover = instance.GetComponent<DataContainerMover>()
                    ?? instance.AddComponent<DataContainerMover>();
        
        // ← 재사용 전 정리!
        mover.CancelMovement();                         // 이전 코루틴 중단
        mover.OnMoveComplete -= OnContainerArrived;     // 이벤트 구독 제거
        
        // 새 이동 초기화
        mover.Initialize(prefab, StartPos, EndPos);
        mover.OnMoveComplete += OnContainerArrived;
    }
    

    private void OnContainerArrived(GameObject instance)
    {
        // 풀에 반환
        ReturnToPool(instance);
        
        // 도착 카운터 감소
        remainingCount--;

        // 마지막 하나가 도착했을 때만 문 닫기
        if (remainingCount <= 0)
        {
            doorAnimator.SetBool(IsOpenHash, false);
        }
    }

    // StageConfig의 dummyEntries, targetEntries → Flat list
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

    // Fisher–Yates 셔플
    private void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // 풀에서 꺼내기 또는 신규 생성
    private GameObject GetFromPool(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var q))
            pools[prefab] = q = new Queue<GameObject>();

        return q.Count > 0 ? q.Dequeue() : Instantiate(prefab);
    }

    // 비활성화 후 풀에 반환
    private void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);

        var mover = instance.GetComponent<DataContainerMover>();
        var prefab = mover.Prefab;
        if (!pools.TryGetValue(prefab, out var q))
            pools[prefab] = q = new Queue<GameObject>();

        q.Enqueue(instance);
    }
}
