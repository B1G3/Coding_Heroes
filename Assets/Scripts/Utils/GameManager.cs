using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;

    public bool isPlacing;
    
    private EnemyPath enemyPath;
    public static GameManager Instance { get; private set; }
    
    private List<StartNode> startNodes = new();

    // [SerializeField] private TMP_Text text;
    void Awake() => Instance = this;
    
    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += SetEnemyPath;
        LeverController.OnLeverMax += LaunchGame;
        leftBlockHolder.OnPlacing += SetPlacementState;
        rightBlockHolder.OnPlacing += SetPlacementState;
    }

    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= SetEnemyPath;
        LeverController.OnLeverMax -= LaunchGame;
        leftBlockHolder.OnPlacing -= SetPlacementState;
        rightBlockHolder.OnPlacing -= SetPlacementState;
    }

    public ITarget GetTarget()
    {
        // text.text += $"Get Target {enemyPath.Position}";
        if(!enemyPath) return null;
        return enemyPath;
    }
    
    public void SaveStartNode(StartNode startNode)
    {
        startNodes.Add(startNode);
    }
    
    public void ClearStartNode()
    {
        startNodes.Clear();
    }

    private void LaunchGame()
    {
        // text.text = "Launch Game";
        enemyPath?.StartGame();
    }

    public void LaunchStartNode()
    {
        foreach (var startNode in startNodes)
        {
            // text.text = $"Launch! {startNode.name}";
            startNode.Launch();
        }
    }

    private void SetEnemyPath(GameObject home)
    {
        enemyPath ??= home.GetComponent<Home>().path1;
        // text.text = $"{enemyPath.Position}";
    }
    
    private void SetPlacementState(bool state)
    {
        isPlacing = state;
    }

}
