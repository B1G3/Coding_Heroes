// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
// public class LogicTriggerManager : MonoBehaviour
// {
//     public static LogicTriggerManager Instance;
//     private void Awake()
//     {
//         Instance = this;
//     }
//
//     public void Trigger(Unit unit, BoxNode startBox)
//     {
//         unit.LoadLogic(startBox);
//         unit.RunCommandsAsync().Forget();
//     }
// }
