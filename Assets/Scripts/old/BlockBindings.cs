using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static BlockConfig;

[Serializable, CreateAssetMenu(
    fileName = "BlockBindings",
    menuName = "Block/Prefab Bindings",
    order = 0)]
public class BlockBindings : ScriptableObject, IEnumerable
{
    public List<BlockTypeBinding> entries  = new();

    public IEnumerator<BlockTypeBinding> GetEnumerator()
    {
        return entries.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}