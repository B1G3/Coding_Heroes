using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlacementManager placementManager;

    private async void Start()
    {
        Debug.Log("[GameManager] Initializing...");

        await inputManager.InitializeAsync();
        await placementManager.InitializeAsync();

        Debug.Log("[GameManager] All systems ready!");
    }

    private void OnDestroy()
    {
        placementManager.Dispose();
        inputManager.Dispose();
    }
}