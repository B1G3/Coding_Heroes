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

    public enum PortType
    {
        Input,
        Output,
        Both,
        None,
    }
    
    // 로컬 방향 (오브젝트 기준)
    public enum LocalDirection
    {
        Forward,    // 오브젝트의 앞 (transform.forward)
        Right,      // 오브젝트의 오른쪽 (transform.right)  
        Back,       // 오브젝트의 뒤 (-transform.forward)
        Left        // 오브젝트의 왼쪽 (-transform.right)
    }

    // 월드 방향 (Origin 기준)
    public enum WorldDirection
    {
        South,      // Origin의 forward 방향 (+Z in origin space)
        East,       // Origin의 right 방향 (+X in origin space)
        North,      // Origin의 back 방향 (-Z in origin space)  
        West        // Origin의 left 방향 (-X in origin space)
    }
}
