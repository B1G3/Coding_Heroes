// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
//
// public class Unit : MonoBehaviour
// {
//     [SerializeField] private float speed = 3f;
//     private List<IUnitCommand> _commands;
//
//
//     public void LoadLogic(BoxNode startBox)
//     {
//         _commands = new List<IUnitCommand>();
//         startBox.ControlModule.Compile(this, _commands);
//     }
//
//
//     public async UniTask RunCommandsAsync()
//     {
//         foreach (var cmd in _commands)
//             await cmd.ExecuteAsync(this);
//     }
//
//     public async UniTask MoveToAsync(Vector3 dest)
//     {
//         while (Vector3.Distance(transform.position, dest) > 0.1f)
//         {
//             transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
//             await UniTask.Yield();
//         }
//     }
// }