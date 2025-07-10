// using System.Collections.Generic;
// using UnityEngine;
//
// public class MoveActionModule : ControlModule
// {
//     [SerializeField] private MonoBehaviour _targetPosInput;
//
//     private IInput<Vector3> TargetPos => (IInput<Vector3>)_targetPosInput;
//
//     public override void Compile(Unit unit, List<IUnitCommand> output)
//     {
//         Vector3 pos = TargetPos.GetValue();
//         output.Add(new MoveCommand(pos));
//     }
// }
