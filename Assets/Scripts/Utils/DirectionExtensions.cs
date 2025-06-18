using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public static class DirectionExtensions
{
    public static Vector3Int ToVector(this Direction dir) => dir switch
    {
        Direction.North => Vector3Int.forward,
        Direction.South => Vector3Int.back,
        Direction.East => Vector3Int.right,
        Direction.West => Vector3Int.left,
        _ => Vector3Int.zero
    };

    public static Direction Opposite(this Direction dir) => dir switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        _ => Direction.None
    };
    
    public static IEnumerable<Direction> AllDirections()
    {
        yield return Direction.North;
        yield return Direction.South;
        yield return Direction.East;
        yield return Direction.West;
    }
}
