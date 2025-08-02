using UnityEngine;
using static BlockConfig;

public static class DirectionUtils
{
    // 로컬 방향을 월드 방향으로 변환 (오브젝트의 rotation 고려)
    public static WorldDirection LocalToWorldDirection(LocalDirection localDir, Transform transform)
    {
        Vector3 localVector = LocalDirectionToVector3(localDir);
        Vector3 worldVector = transform.TransformDirection(localVector);
        return WorldVectorToDirection(worldVector);
    }
    
    // 월드 방향을 로컬 방향으로 변환
    public static LocalDirection WorldToLocalDirection(WorldDirection worldDir, Transform transform)
    {
        Vector3 worldVector = WorldDirectionToVector3(worldDir);
        Vector3 localVector = transform.InverseTransformDirection(worldVector);
        return Vector3ToLocalDirection(localVector);
    }
    
    // 로컬 방향을 Vector3로 변환
    public static Vector3 LocalDirectionToVector3(LocalDirection direction)
    {
        return direction switch
        {
            LocalDirection.Forward => Vector3.forward,
            LocalDirection.Right => Vector3.right,
            LocalDirection.Back => Vector3.back,
            LocalDirection.Left => Vector3.left,
            _ => Vector3.forward
        };
    }
    
    // 월드 방향을 Origin 기준 Vector3로 변환
    public static Vector3 WorldDirectionToVector3(WorldDirection direction)
    {
        return direction switch
        {
            WorldDirection.South => Vector3.forward,    // Origin의 forward
            WorldDirection.East => Vector3.right,       // Origin의 right
            WorldDirection.North => Vector3.back,       // Origin의 back
            WorldDirection.West => Vector3.left,        // Origin의 left
            _ => Vector3.forward
        };
    }
    
    // Origin 기준 Vector3를 월드 방향으로 변환
    public static WorldDirection WorldVectorToDirection(Vector3 vector)
    {
        // Origin 공간에서의 벡터로 변환
        Transform origin = GridUtils.GetOrigin();
        Vector3 originSpaceVector = origin.InverseTransformDirection(vector);
        
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
    
    // Vector3를 로컬 방향으로 변환
    public static LocalDirection Vector3ToLocalDirection(Vector3 vector)
    {
        float absX = Mathf.Abs(vector.x);
        float absZ = Mathf.Abs(vector.z);
        
        if (absX > absZ)
        {
            return vector.x > 0 ? LocalDirection.Right : LocalDirection.Left;
        }
        else
        {
            return vector.z > 0 ? LocalDirection.Forward : LocalDirection.Back;
        }
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
    
    // Vector3Int를 월드 방향으로 변환 (그리드 이동용)
    public static WorldDirection Vector3IntToWorldDirection(Vector3Int vector)
    {
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.z))
        {
            return vector.x > 0 ? WorldDirection.East : WorldDirection.West;
        }
        else
        {
            return vector.z > 0 ? WorldDirection.South : WorldDirection.North;
        }
    }
    
    // 월드 방향을 Vector3Int로 변환 (그리드 이동용)
    public static Vector3Int WorldDirectionToVector3Int(WorldDirection direction)
    {
        return direction switch
        {
            WorldDirection.South => Vector3Int.forward,
            WorldDirection.North => Vector3Int.back,
            WorldDirection.East => Vector3Int.right,
            WorldDirection.West => Vector3Int.left,
            _ => Vector3Int.forward
        };
    }
}