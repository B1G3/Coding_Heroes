using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;
    
    [Header("Block Reposition")]
    [SerializeField] private BlockReposition leftBlockReposition;
    [SerializeField] private BlockReposition rightBlockReposition;
    
    [Header("Voice Recoder")]
    [SerializeField] private VoiceRecoder voiceRecoder;
    
    [Header("Stage Data")]
    [SerializeField] private StageConfig currentStage; 
    
    public enum GameStateType
    {
        Idle,
        Road,
        Reposition,
        Record,
    }
    
    private IGameState currentState;
    private RoadState roadState;
    private IdleGameState idleGameState;
    private RecordState recordState;
    private RepositionState repositionState;
    
    private List<EnemyPath> enemyPath;
    public static GameManager Instance { get; private set; }
    
    private List<StartNode> startNodes = new();

    // [SerializeField] private TMP_Text text;
    void Awake() => Instance = this;

    private void Start()
    {
        idleGameState = new IdleGameState();
        idleGameState.Entered += leftBlockHolder.Enter;
        idleGameState.Entered += rightBlockHolder.Enter;
        idleGameState.Exited += leftBlockHolder.Exit;
        idleGameState.Exited += rightBlockHolder.Exit;
        
        recordState = new RecordState();
        recordState.Entered += voiceRecoder.Enter;
        recordState.Exited += voiceRecoder.Exit;
        
        repositionState = new RepositionState();
        repositionState.Entered += leftBlockReposition.Enter;
        repositionState.Entered += rightBlockReposition.Enter;
        repositionState.Exited += leftBlockReposition.Exit;
        repositionState.Exited += rightBlockReposition.Exit;
        
        roadState = new RoadState();
        roadState.Entered += leftBlockHolder.Enter;
        roadState.Entered += rightBlockHolder.Enter;
        roadState.Exited += leftBlockHolder.Exit;
        roadState.Exited += rightBlockHolder.Exit;
        roadState.Entered += leftBlockHolder.PlacePathEnter;
        roadState.Entered += rightBlockHolder.PlacePathEnter;
        roadState.Exited += leftBlockHolder.PlacePathExit;
        roadState.Exited += rightBlockHolder.PlacePathExit;
        
        SetState(idleGameState);
    }

    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += SetEnemyPath;
        LeverController.OnLeverMax += LaunchGame;
        StageManager.OnStageChanged += SetStage;
    }

    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= SetEnemyPath;
        LeverController.OnLeverMax -= LaunchGame;
        StageManager.OnStageChanged -= SetStage;
    }
    
    private void SetState(IGameState newState)
    {
        if (currentState == newState) return;
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    
    public void ChangeState(GameStateType type)
    {
        switch (type)
        {
            case GameStateType.Idle:
                SetState(idleGameState);
                break;
            case GameStateType.Record:
                SetState(recordState);
                break;
            case GameStateType.Reposition:
                SetState(repositionState);
                break;
            case GameStateType.Road:
                SetState(roadState);
                break;
            default:
                break;
        }
    }
    
    public bool CanPlaceBlock() => currentState?.CanPlaceBlock ?? false;
    public bool CanRotateBlock() => currentState?.CanRotateBlock ?? false;
    public bool CanReposition() => currentState?.CanReposition ?? false;

    public ITarget GetTarget(int index)
    {
        // text.text += $"Get Target {enemyPath.Position}";
        if(!enemyPath[index]) return null;
        return enemyPath[index];
    }
    
    public void SaveStartNode(StartNode startNode)
    {
        startNodes.Add(startNode);
    }
    
    public void ClearStartNode()
    {
        startNodes.Clear();
    }
    
    private void SetStage(StageConfig stage)
    {
        currentStage = stage;
    }

    private void LaunchGame()
    {
        // text.text = "Launch Game";
        enemyPath[0]?.StartGame(currentStage);
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
        enemyPath ??= home.GetComponent<Home>().path;
        // text.text = $"{enemyPath.Position}";
    }
}
