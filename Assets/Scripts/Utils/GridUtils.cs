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
    
    // Origin 접근자 추가
    public static Transform GetOrigin()
    {
        return _origin;
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

    // 스냅 계산 (직선 보정) - 월드 방향 기준으로 수정
    public static Vector3Int SnapToStraight(Vector3 startWorld, Vector3 targetWorld)
    {
        var fromCell = WorldToCell(startWorld);
        var targetCell = WorldToCell(targetWorld);

        // Origin 공간에서의 차이 계산
        Vector3 originSpaceDiff = _origin.InverseTransformVector(targetWorld - startWorld);
        
        int dx = Mathf.Abs(Mathf.RoundToInt(originSpaceDiff.x / _cellSize.x));
        int dz = Mathf.Abs(Mathf.RoundToInt(originSpaceDiff.z / _cellSize.z));

        var resultCell = fromCell;
        if (dx > dz) 
            resultCell.x = targetCell.x;
        else         
            resultCell.z = targetCell.z;

        return resultCell;
    }

    public static Quaternion GetOriginRotation()
    {
        return _origin.rotation;
    }
    
    // Origin 기준 방향 벡터들
    public static Vector3 GetOriginForward() => _origin.forward;    // South
    public static Vector3 GetOriginRight() => _origin.right;       // East  
    public static Vector3 GetOriginBack() => -_origin.forward;     // North
    public static Vector3 GetOriginLeft() => -_origin.right;       // West
    
    // 월드 위치를 Origin 로컬 공간으로 변환
    public static Vector3 WorldToOriginLocal(Vector3 worldPos)
    {
        return _origin.InverseTransformPoint(worldPos);
    }
    
    // Origin 로컬 공간을 월드 위치로 변환
    public static Vector3 OriginLocalToWorld(Vector3 localPos)
    {
        return _origin.TransformPoint(localPos);
    }
    
    // Origin 기준 방향 벡터를 월드 공간으로 변환
    public static Vector3 OriginDirectionToWorld(Vector3 originDirection)
    {
        return _origin.TransformDirection(originDirection);
    }
}