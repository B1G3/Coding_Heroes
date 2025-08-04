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
    
    public static Vector3Int SnapToDirection(Vector3 startWorld, Vector3 targetWorld, BlockConfig.WorldDirection outputDirection)
    {
        var fromCell = WorldToCell(startWorld);
        var targetCell = WorldToCell(targetWorld);
    
        Vector3Int resultCell = fromCell;
    
        switch (outputDirection)
        {
            case BlockConfig.WorldDirection.East: // X+ 방향만
                if (targetCell.x > fromCell.x)
                    resultCell.x = targetCell.x;
                break;
            
            case BlockConfig.WorldDirection.West: // X- 방향만
                if (targetCell.x < fromCell.x)
                    resultCell.x = targetCell.x;
                break;
            
            case BlockConfig.WorldDirection.South: // Z+ 방향만
                if (targetCell.z > fromCell.z)
                    resultCell.z = targetCell.z;
                break;
            
            case BlockConfig.WorldDirection.North: // Z- 방향만
                if (targetCell.z < fromCell.z)
                    resultCell.z = targetCell.z;
                break;
        }
    
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
    
    public static Quaternion RotationFromWorldDirection(BlockConfig.WorldDirection dir)
    {
        if (_origin == null) return Quaternion.identity;

        // WorldDirection을 월드 공간 벡터로
        Vector3 worldDir = DirectionUtils.GetWorldDirectionVector(dir);
        // origin 로컬 공간 기준 벡터를 월드로 변환한 뒤 LookRotation
        Vector3 forward = _origin.TransformDirection(Vector3.forward);
        // worldDir은 origin 기준이 아니라 실제 월드 방향 벡터가 필요하므로
        // DirectionUtils.GetWorldDirectionVector는 origin 기준으로 해석된 벡터라면 그대로 씀
        Vector3 targetDir = worldDir.normalized;
        if (targetDir.sqrMagnitude < 0.0001f) targetDir = forward;

        // y축만 회전하게끔, 위로 벡터를 그대로 유지
        return Quaternion.LookRotation(targetDir, Vector3.up);
    }
}