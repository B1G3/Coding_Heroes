using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance { get; private set; }

    private Dictionary<Vector3Int, BoxBase> grid = new();

    void Awake() => Instance = this;

    public void RegisterBox(BoxBase box) {
        grid[box.GridPosition] = box;
    }

    public T FindNearbyInterface<T>(Vector3Int pos) where T : class {
        Vector3Int[] directions = {
            Vector3Int.left, Vector3Int.right,
            Vector3Int.forward, Vector3Int.back
        };

        foreach (var dir in directions) {
            var neighbor = pos + dir;
            if (grid.TryGetValue(neighbor, out var box)) {
                if (box is T target) {
                    return target;
                }
            }
        }
        return null;
    }

    void Update() {
        foreach (var box in grid.Values) {
            box.Tick(); // 모든 박스의 행동 처리
        }
    }
}
