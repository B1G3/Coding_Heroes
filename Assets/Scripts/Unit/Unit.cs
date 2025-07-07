using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public void StartUnit(List<string> command)
    {
        foreach (var c in command)
        {
            Debug.Log(c);
        }
    }
}
