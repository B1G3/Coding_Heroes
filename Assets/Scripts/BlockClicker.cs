using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static BlockConfig;

public class BlockClicker : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private BlockType blockType;
    public UnityEvent<GameObject> onClick;

    public BlockType GetBlockType()
    {
        return blockType;
    }
    
    public GameObject GetBlock()
    {
        return blockPrefab;
    }
}