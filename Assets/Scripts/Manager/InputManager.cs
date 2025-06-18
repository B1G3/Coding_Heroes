using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;

    public InputAction PlaceAction { get; private set; }
    public InputAction CancelAction { get; private set; }
    public Vector2 MousePosition => Mouse.current.position.ReadValue();
    
    public enum InputContext
    {
        Gameplay,
        UI
    }
    
    private InputActionMap gameplayMap;
    private InputActionMap uiMap;

    public async UniTask InitializeAsync()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameplayMap = inputActions.FindActionMap("Gameplay");
        uiMap = inputActions.FindActionMap("UI");

        PlaceAction = gameplayMap.FindAction("Place");
        CancelAction = gameplayMap.FindAction("Cancel");
        
        PlaceAction.Enable();
        CancelAction.Enable();

        SwitchContext(InputContext.Gameplay);
        await UniTask.Yield();
    }

    public void Dispose()
    {
        PlaceAction.Disable();
        CancelAction.Disable();
    }
    
    public void SwitchContext(InputContext context)
    {
        gameplayMap.Disable();
        uiMap.Disable();

        switch (context)
        {
            case InputContext.Gameplay:
                gameplayMap.Enable();
                break;
            case InputContext.UI:
                uiMap.Enable();
                break;
        }
    }
}