using UnityEngine;

public static class GridUtils
{
    private static Transform _origin;
    private static Vector3 _cellSize;

    public static void Initialize(Transform origin, Vector3 cellSize)
    {
        _origin = origin;
        _cellSize = cellSize;
    }
    // world → cell
    public static Vector3Int WorldToCell(Vector3 worldPos)
    {
        Vector3 localPos = _origin.InverseTransformPoint(worldPos);
        return new Vector3Int(
            Mathf.RoundToInt(localPos.x / _cellSize.x),
            Mathf.RoundToInt(localPos.y / _cellSize.y),
            Mathf.RoundToInt(localPos.z / _cellSize.z)
        );
    }

    // cell → world
    public static Vector3 CellToWorld(Vector3Int cell)
    {
        Vector3 localCenter = new Vector3(
            cell.x * _cellSize.x,
            cell.y * _cellSize.y,
            cell.z * _cellSize.z
        );
        return _origin.TransformPoint(localCenter);
    }

    // 스냅 계산 (직선 보정)
    public static Vector3Int SnapToStraight(
        Vector3 startWorld, Vector3 targetWorld
    )
    {
        var fromCell   = WorldToCell(startWorld);
        var targetCell = WorldToCell(targetWorld);

        int dx = Mathf.Abs(targetCell.x - fromCell.x);
        int dz = Mathf.Abs(targetCell.z - fromCell.z);

        var resultCell = fromCell;
        if (dx > dz) resultCell.x = targetCell.x;
        else         resultCell.z = targetCell.z;

        return resultCell;
    }

    public static Quaternion GetOriginRotation()
    {
        return _origin.rotation;
    }
}
