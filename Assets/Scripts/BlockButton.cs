using UnityEngine;
using static BlockConfig;

public class BlockButton : MonoBehaviour
{
    [SerializeField] private BlockType blockType;

    public void OnClick()
    {
        BlockSelection.Select(blockType);
    }
}
