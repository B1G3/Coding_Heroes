// using System;
// using UnityEngine;
// using Cysharp.Threading.Tasks;
//
// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance { get; private set; }
//     
//     [SerializeField] private InputManager inputManager;
//     [SerializeField] private PlacementManager placementManager;
//     
//     // 임시값
//     [SerializeField] private StartNode startNode;
//     
//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//
//         Instance = this;
//         DontDestroyOnLoad(gameObject);
//     }
//
//     private async void Start()
//     {
//         try
//         {
//             Debug.Log("[GameManager] Initializing...");
//
//             await inputManager.InitializeAsync();
//             await placementManager.InitializeAsync();
//             
//             //임시 초기화
//             startNode.InitializeAsync().Forget();
//
//             Debug.Log("[GameManager] All systems ready!");
//         }
//         catch (Exception e)
//         {
//             Debug.LogError(e);
//         }
//     }
//
//     private void OnDestroy()
//     {
//         placementManager.Dispose();
//         inputManager.Dispose();
//     }
// }