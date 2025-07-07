using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private IUnitState CurrentState;
    [SerializeField] private float speed = 3f;
    public float Speed => speed;
    
    public void StartUnit(List<string> command)
    {
        foreach (var c in command)
        {
            Debug.Log(c);
        }
    }
    
    
    public void ChangeState(IUnitState state)
    {
        CurrentState?.Exit(this);
        CurrentState = state;
        CurrentState?.Enter(this);
    }
}
