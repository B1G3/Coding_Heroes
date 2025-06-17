using UnityEngine;
using static Const;

public class BlockButton : MonoBehaviour
{
    [SerializeField] private BlockType blockType;

    public void OnClick()
    {
        BlockSelection.Select(blockType);
    }
}
