using UnityEngine;
using static BlockConfig;

public class BlockButton : MonoBehaviour
{
    [SerializeField] private BlockType blockType;
    [SerializeField] private int index;

    public void OnClick()
    {
        BlockSelection.Select(blockType, index);
    }
}