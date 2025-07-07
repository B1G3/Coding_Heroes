using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUnitCommand
{
    UniTask ExecuteAsync(Unit unit);
}
