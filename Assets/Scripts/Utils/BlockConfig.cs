using System;
using UnityEngine;

public class BlockConfig
{
    [Serializable]
    public struct BlockPrefabBinding
    {
        public BlockType type;
        public GameObject prefab;
        public GameObject uiPrefab;
        public LayerMask ignoredLayers;
    }
    
    public enum BlockType
    {
        None,
        Tree,
        Harvester,
        Storage,
        Rail,
        
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