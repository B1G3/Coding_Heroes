using UnityEngine;

public class ConstantInput<T> : MonoBehaviour, IInput<T>
{
    [SerializeField] private T _value;
    public T GetValue() => _value;
}
