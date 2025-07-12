using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BlockClicker : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    public UnityEvent<GameObject> onClick;

    public GameObject GetBlock()
    {
        return blockPrefab;
    }
}