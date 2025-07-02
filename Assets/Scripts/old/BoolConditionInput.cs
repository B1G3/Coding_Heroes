using UnityEngine;

public class BoolConditionInput : MonoBehaviour, IInput<bool>
{
    [SerializeField] private ConditionModule _conditionModule;
    public bool GetValue() => _conditionModule != null && _conditionModule.Evaluate();
}