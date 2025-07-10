// using System;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
// public class StartNode : MonoBehaviour
// {
//     [Header("First BoxNode")]
//     [SerializeField] private Block firstBox;
//
//     public Block FirstBox => firstBox;
//     
//     public async UniTaskVoid InitializeAsync()
//     {
//         Vector3Int gridPos = Vector3Int.RoundToInt(transform.position);
//         firstBox.Initialize(gridPos);
//         await UniTask.Yield();
//     }
// }
