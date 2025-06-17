using UnityEngine;

public class BlockPreview : MonoBehaviour
{
    public void SetValid(bool isValid)
    {
        var color = isValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
        }
    }
}
