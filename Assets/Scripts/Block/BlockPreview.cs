using UnityEngine;

public class BlockPreview : MonoBehaviour
{
    public void SetValid(bool isValid)
    {
        var color = isValid ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.2f);
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
        }
    }
}