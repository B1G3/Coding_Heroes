// using UnityEngine;
// using static BlockConfig;
//
// public class ConnectionUtil
// {
//     public static void TryConnect(BoxBase from, BoxBase to, Direction flowDirection)
//     {
//         if (from is IOutput output && to is IInput input)
//         {
//             if (output.CanSend(flowDirection) && input.CanReceive(flowDirection))
//             {
//                 output.RegisterOutputTarget(input, flowDirection);
//                 
//                 if (output is { } outDirSetter)
//                     outDirSetter.SetOutputDirection(flowDirection);
//
//                 if (input is { } inDirSetter)
//                     inDirSetter.SetInputDirection(flowDirection);
//                 
//                 Debug.Log($"[Connection] Connected from {from} to {to} via {flowDirection}");
//             }
//             else
//             {
//                 Debug.LogWarning("[Connection] Direction mismatch or blocked.");
//             }
//         }
//     }
// }
