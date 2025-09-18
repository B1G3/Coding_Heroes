using System;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private LexyDialogue lexyDialogue;
    public List<EnemyPath> path;

    public void Initialize()
    {
        DialogueManager.Instance.SetLexyDialogue(lexyDialogue);
    }
}
