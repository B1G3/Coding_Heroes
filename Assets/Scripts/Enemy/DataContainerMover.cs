using System;
using System.Collections;
using UnityEngine;

public class DataContainerMover : MonoBehaviour
{
    private Vector3 _targetPosition;
    private float _speed = 0.1f;
    
    public event Action OnMoveComplete;

    /// <summary>
    /// 웜 이동 초기화
    /// </summary>
    public void Initialize(Vector3 start, Vector3 end)
    {
        transform.position  = start;
        _targetPosition     = end;

        // ▶ start → end 방향을 바라보도록
        transform.LookAt(_targetPosition);

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (Vector3.Distance(transform.position, _targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                _speed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = _targetPosition;
        OnMoveComplete?.Invoke();
        Destroy(gameObject);
    }
}
