using UnityEngine;

public class GridDirector : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector3 cellSize = new Vector3(3, 3, 3);

    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += OnHomeSpawned;
    }

    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= OnHomeSpawned;
    }

    private void OnHomeSpawned(GameObject home)
    {
        // 한 번만 초기화
        GridUtils.Initialize(home.transform, cellSize);
    }
}