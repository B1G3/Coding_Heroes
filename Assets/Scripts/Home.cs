using System;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private LexyDialogue lexyDialogue;
    public EnemyPath path1;

    public void Initialize(DialogueNode[] nodes)
    {
        lexyDialogue.Initialize(nodes);
    }
}
