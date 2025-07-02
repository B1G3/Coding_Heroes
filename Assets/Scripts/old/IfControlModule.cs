using System.Collections.Generic;
using UnityEngine;

public class IfControlModule : ControlModule
{
    [SerializeField] private MonoBehaviour _conditionInput; // assign IInput<bool>
    [SerializeField] private ControlModule _trueBranch;
    [SerializeField] private ControlModule _falseBranch;

    private IInput<bool> Condition => (IInput<bool>)_conditionInput;

    public override void Compile(Unit unit, List<IUnitCommand> output)
    {
        if (Condition.GetValue())
            _trueBranch?.Compile(unit, output);
        else
            _falseBranch?.Compile(unit, output);
    }
}
