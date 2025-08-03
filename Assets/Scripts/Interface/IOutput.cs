using UnityEngine;

public interface IOutput  
{
    BlockConfig.WorldDirection GetOutputDirection(Transform transform);
}