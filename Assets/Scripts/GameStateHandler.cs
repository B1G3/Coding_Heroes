using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStateHandler : MonoBehaviour
{
    [SerializeField] private List<Image> stateImages;
    [SerializeField] private List<Sprite> stateOn;
    [SerializeField] private List<Sprite> stateOff;
    [SerializeField] private GameObject userPanel;

    private bool[] isOn = new bool[4];
    
    private void Start()
    {
        userPanel.SetActive(false);
    }
    
    public void SetState(int index)
    {
        if(IsAnyOtherActive(index)) return;
        isOn[index] = !isOn[index];
        int state = isOn[index] ? index : 0;
        if (index == 3) userPanel.SetActive(isOn[index]);
        stateImages[index].sprite = isOn[index] ? stateOn[index] : stateOff[index];
        GameManager.Instance.ChangeState((GameManager.GameStateType)state);
    }
    
    private bool IsAnyOtherActive(int index) =>
        isOn
            .Where((on, i) => i != index)
            .Any(on => on);
}
