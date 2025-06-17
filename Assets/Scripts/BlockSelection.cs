using System;
using UnityEngine;
using static Const;

public static class BlockSelection
{
    public static event Action<BlockType> OnBlockTypeSelected;

    private static BlockType current = BlockType.None;

    public static BlockType Current => current;

    public static void Select(BlockType type)
    {
        if (current == type) return;

        current = type;
        OnBlockTypeSelected?.Invoke(current);
        Debug.Log($"[BlockSelection] Selected block: {type}");
    }

    public static void Clear()
    {
        Select(BlockType.None);
    }
}