using UnityEngine;
using static BlockConfig;

public static class DirectionUtils
{
    // 로컬 방향을 월드 방향으로 변환 (Origin 기준)
    public static WorldDirection LocalToWorldDirection(LocalDirection localDir)
    {
        // Origin의 회전을 고려해서 변환
        Vector3 localVector = LocalDirectionToOriginSpaceVector(localDir);
        Vector3 worldVector = GridUtils.OriginDirectionToWorld(localVector);
        return WorldVectorToDirection(worldVector);
    }
    
    // 월드 방향을 로컬 방향으로 변환 (Origin 기준)
    public static LocalDirection WorldToLocalDirection(WorldDirection worldDir)
    {
        Vector3 worldVector = GetWorldDirectionVector(worldDir);
        Vector3 originSpaceVector = GridUtils.GetOrigin().InverseTransformDirection(worldVector);
        return OriginSpaceVectorToLocalDirection(originSpaceVector);
    }
    
    // 로컬 방향을 Origin 로컬 공간의 Vector3로 변환
    private static Vector3 LocalDirectionToOriginSpaceVector(LocalDirection direction)
    {
        return direction switch
        {
            LocalDirection.Forward => Vector3.forward,   // Origin의 forward 방향
            LocalDirection.Right => Vector3.right,       // Origin의 right 방향
            LocalDirection.Back => Vector3.back,         // Origin의 back 방향
            LocalDirection.Left => Vector3.left,         // Origin의 left 방향
            _ => Vector3.forward
        };
    }
    
    // Origin 로컬 공간 벡터를 로컬 방향으로 변환
    private static LocalDirection OriginSpaceVectorToLocalDirection(Vector3 originSpaceVector)
    {
        float absX = Mathf.Abs(originSpaceVector.x);
        float absZ = Mathf.Abs(originSpaceVector.z);
        
        if (absX > absZ)
        {
            return originSpaceVector.x > 0 ? LocalDirection.Right : LocalDirection.Left;
        }
        else
        {
            return originSpaceVector.z > 0 ? LocalDirection.Forward : LocalDirection.Back;
        }
    }
    
    // 월드 방향을 실제 월드 벡터로 변환 (GridUtils 활용)
    public static Vector3 GetWorldDirectionVector(WorldDirection direction)
    {
        return direction switch
        {
            WorldDirection.South => GridUtils.GetOriginForward(),   // +Z in origin space
            WorldDirection.East => GridUtils.GetOriginRight(),      // +X in origin space
            WorldDirection.North => GridUtils.GetOriginBack(),      // -Z in origin space
            WorldDirection.West => GridUtils.GetOriginLeft(),       // -X in origin space
            _ => GridUtils.GetOriginForward()
        };
    }
    
    // 월드 벡터를 월드 방향으로 변환
    public static WorldDirection WorldVectorToDirection(Vector3 worldVector)
    {
        Transform origin = GridUtils.GetOrigin();
        if (origin == null) return WorldDirection.South;
        
        // 월드 벡터를 Origin 로컬 공간으로 변환
        Vector3 originSpaceVector = origin.InverseTransformDirection(worldVector);
        
        // 가장 큰 성분을 기준으로 방향 결정
        float absX = Mathf.Abs(originSpaceVector.x);
        float absZ = Mathf.Abs(originSpaceVector.z);
        
        if (absX > absZ)
        {
            return originSpaceVector.x > 0 ? WorldDirection.East : WorldDirection.West;
        }
        else
        {
            return originSpaceVector.z > 0 ? WorldDirection.South : WorldDirection.North;
        }
    }
    
    // 그리드 위치 간의 방향 계산 (Vector3Int 기반)
    public static WorldDirection GetDirectionBetweenGridPositions(Vector3Int from, Vector3Int to)
    {
        Vector3Int diff = to - from;
        
        // 가장 큰 차이를 가진 축으로 방향 결정
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            return diff.x > 0 ? WorldDirection.East : WorldDirection.West;
        }
        else
        {
            return diff.z > 0 ? WorldDirection.South : WorldDirection.North;
        }
    }
    
    // 월드 방향을 그리드 이동 벡터로 변환
    public static Vector3Int WorldDirectionToGridVector(WorldDirection direction)
    {
        return direction switch
        {
            WorldDirection.South => Vector3Int.forward,  // +Z
            WorldDirection.North => Vector3Int.back,     // -Z
            WorldDirection.East => Vector3Int.right,     // +X
            WorldDirection.West => Vector3Int.left,      // -X
            _ => Vector3Int.forward
        };
    }
    
    // 그리드 벡터를 월드 방향으로 변환
    public static WorldDirection GridVectorToWorldDirection(Vector3Int gridVector)
    {
        if (Mathf.Abs(gridVector.x) > Mathf.Abs(gridVector.z))
        {
            return gridVector.x > 0 ? WorldDirection.East : WorldDirection.West;
        }
        else
        {
            return gridVector.z > 0 ? WorldDirection.South : WorldDirection.North;
        }
    }
    
    // 그리드 위치에서 특정 방향으로 한 칸 이동한 위치 계산
    public static Vector3Int MoveGridPosition(Vector3Int currentPos, WorldDirection direction)
    {
        return currentPos + WorldDirectionToGridVector(direction);
    }
    
    // 두 그리드 위치가 인접한지 확인
    public static bool AreGridPositionsAdjacent(Vector3Int pos1, Vector3Int pos2)
    {
        Vector3Int diff = pos1 - pos2;
        int distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.z);
        return distance == 1 && diff.y == 0; // 같은 높이에서 1칸 차이
    }
    
    // 반대 방향 구하기
    public static LocalDirection GetOppositeLocalDirection(LocalDirection direction)
    {
        return direction switch
        {
            LocalDirection.Forward => LocalDirection.Back,
            LocalDirection.Back => LocalDirection.Forward,
            LocalDirection.Right => LocalDirection.Left,
            LocalDirection.Left => LocalDirection.Right,
            _ => LocalDirection.Forward
        };
    }
    
    public static WorldDirection GetOppositeWorldDirection(WorldDirection direction)
    {
        return direction switch
        {
            WorldDirection.North => WorldDirection.South,
            WorldDirection.South => WorldDirection.North,
            WorldDirection.East => WorldDirection.West,
            WorldDirection.West => WorldDirection.East,
            _ => WorldDirection.South
        };
    }
    
    // 연결 가능한 방향인지 확인 (Output -> Input 연결)
    public static bool CanConnect(Vector3Int outputPos, WorldDirection outputDir, Vector3Int inputPos, WorldDirection inputDir)
    {
        // 출력 위치에서 출력 방향으로 한 칸 이동한 곳이 입력 위치와 같은지 확인
        Vector3Int expectedInputPos = MoveGridPosition(outputPos, outputDir);
        if (expectedInputPos != inputPos) return false;
        
        // 입력 방향이 출력 방향의 반대인지 확인
        return inputDir == GetOppositeWorldDirection(outputDir);
    }
}