using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [SerializeField] private bool isOn;
    public void SetState(int index)
    {
        int condition = isOn ? 0 : index;
        GameManager.Instance.ChangeState((GameManager.GameStateType)condition);
        isOn = !isOn;
    }
}
