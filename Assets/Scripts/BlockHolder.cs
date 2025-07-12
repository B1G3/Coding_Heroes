using TMPro;
using UnityEngine;

public class BlockHolder : MonoBehaviour
{
    private GameObject currentBlock;
    [SerializeField] private TMP_Text _text;

    private void OnTriggerEnter(Collider other)
    {
        currentBlock ??= other.GetComponent<BlockClicker>()?.GetBlock();
        _text.text = $"{currentBlock.name}";
    }
}
