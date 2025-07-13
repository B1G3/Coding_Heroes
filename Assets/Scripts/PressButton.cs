using UnityEngine;

public class PressButton : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.LaunchStartNode();
    }
}
