using System.Collections.Generic;
using UnityEngine;

public class InvokeBounce : MonoBehaviour
{
    [SerializeField] private List<RandomBounce> bounces;

    public void Bounce()
    {
        foreach (var bounce in bounces)
        {
            bounce.InvokeBounce();
        }
    }
}
