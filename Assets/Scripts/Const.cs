using System;
using UnityEngine;

public class Const
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
    }
}
