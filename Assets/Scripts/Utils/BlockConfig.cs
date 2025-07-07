using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockConfig
{
    [Serializable]
    public struct BlockTypeBinding
    {
        public BlockType type;
        public List<PrefabEntry> prefabs;
    }

    [Serializable]
    public struct PrefabEntry
    {
        public GameObject prefab;              
        public LayerMask ignoredLayers;        
    }
    
    public enum BlockType
    {
        Block,
        Path,
        Unit,
        Data,
        None,
        
    }
    
    public enum Direction
    {
        None,
        North,
        East,
        South,
        West
    }
}
