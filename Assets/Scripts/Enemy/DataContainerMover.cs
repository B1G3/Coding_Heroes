using System;
using System.Collections;
using UnityEngine;

public class DataContainerMover : MonoBehaviour
{
    // 원본 프리팹 레퍼런스 (풀 복귀 시 사용)
    public GameObject Prefab { get; private set; }

    private Vector3 _targetPosition;
    private float _speed = 0.1f;
    private bool _cancelled = false;
    private Coroutine _moveCoroutine;

    public event Action<GameObject> OnMoveComplete;

    /// <summary>
    /// 웜 이동 초기화 (프리팹, 시작 위치, 도착 위치)
    /// </summary>
    public void Initialize(GameObject prefab, Vector3 start, Vector3 end)
    {
        Prefab           = prefab;          // 풀 식별용 원본 프리팹 저장
        transform.position = start;
        _targetPosition    = end;
        _cancelled = false;

        // ▶ start → end 방향을 바라보도록
        transform.LookAt(_targetPosition);

        // 이동 시작
        _moveCoroutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (!_cancelled && Vector3.Distance(transform.position, _targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                _speed * Time.deltaTime
            );
            yield return null;
        }

        if (_cancelled)
            yield break;

        transform.position = _targetPosition;
        OnMoveComplete?.Invoke(gameObject);
        // Destroy 제거: 풀로 복귀 처리로 대체
    }

    /// <summary>
    /// 외부에서 호출해서 이동을 즉시 멈추도록
    /// </summary>
    public void CancelMovement()
    {
        _cancelled = true;
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
    }
}