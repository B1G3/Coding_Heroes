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
    
    [Header("Input")]
    [SerializeField] private InputActionReference leftGrabAction;  
    [SerializeField] private InputActionReference rightGrabAction; 
    
    [Header("Stage Data")]
    [SerializeField] private StageConfig currentStage; 
    
    public enum GameStateType
    {
        Idle,
        Road,
        Reposition,
        Record,
        Start,
    }
    
    private IGameState currentState;
    private StartGameState startGameState;
    private IdleGameState idleGameState;
    private RecordState recordState;
    private RepositionState repositionState;
    
    private EnemyPath enemyPath;
    public static GameManager Instance { get; private set; }
    
    private List<StartNode> startNodes = new();

    // [SerializeField] private TMP_Text text;
    void Awake() => Instance = this;

    private void Start()
    {
        startGameState = new StartGameState();
        startGameState.Entered += Enter;
        startGameState.Exited += Exit;
        
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

    private void Enter()
    {
        leftGrabAction.action.Enable();
        rightGrabAction.action.Enable();
    }
    
    private void Exit()
    {
        leftGrabAction.action.Disable();
        rightGrabAction.action.Disable();
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
            default:
                break;
        }
    }
    
    public bool CanPlaceBlock() => currentState?.CanPlaceBlock ?? false;
    public bool CanRotateBlock() => currentState?.CanRotateBlock ?? false;
    public bool CanReposition() => currentState?.CanReposition ?? false;

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
    
    private void SetStage(StageConfig stage)
    {
        currentStage = stage;
    }

    private void LaunchGame()
    {
        // text.text = "Launch Game";
        enemyPath?.StartGame(currentStage);
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
}
