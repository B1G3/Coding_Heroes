using System;
using UnityEngine;
using static BlockConfig;

public static class BlockSelection
{
    public static event Action<BlockType, int> OnBlockTypeSelected;

    private static BlockType current = BlockType.None;
    private static int currentIndex = -1;

    public static void Select(BlockType type, int index = -1)
    {
        if (current == type && currentIndex == index) return;

        current = type;
        currentIndex = index;
        OnBlockTypeSelected?.Invoke(current, currentIndex);
        // Debug.Log($"[BlockSelection] Selected block: {type}");
    }

    public static void Clear()
    {
        Select(BlockType.None);
    }
}