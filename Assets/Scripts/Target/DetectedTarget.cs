using UnityEngine;

public class DetectedTarget : ITarget
{
    private GameObject detectedObject;
    
    public DetectedTarget(GameObject obj)
    {
        detectedObject = obj;
    }
    
    public Vector3 Position => detectedObject != null ? detectedObject.transform.position : Vector3.zero;
    
    public GameObject GameObject => detectedObject;
    
    public bool IsValid => detectedObject != null;
}