// using System.Collections.Generic;
// using UnityEngine;
//
// public class ForControlModule : ControlModule
// {
//     [SerializeField] private MonoBehaviour _countInput; // assign IInput<int>
//     [SerializeField] private ControlModule _body;
//
//     private IInput<int> Count => (IInput<int>)_countInput;
//
//     public override void Compile(Unit unit, List<IUnitCommand> output)
//     {
//         int n = Count.GetValue();
//         for (int i = 0; i < n; i++)
//             _body?.Compile(unit, output);
//     }
// }
