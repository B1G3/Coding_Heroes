using UnityEngine;

public interface IInput
{
    BlockConfig.WorldDirection GetInputDirection(Transform transform);
}