using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockConfig
{
    [Serializable]
    public struct BlockTypeBinding
    {
        public BlockType type;                 // → Tree, Harvester…
        public List<PrefabEntry> prefabs;      // → 한 타입에 여러 프리팹
    }

    [Serializable]
    public struct PrefabEntry
    {
        public GameObject prefab;              
        public GameObject uiPrefab;            
        public LayerMask ignoredLayers;        
    }
    
    public enum BlockType
    {
        None,
        Box,
        Rail,
        Unit,
        
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
