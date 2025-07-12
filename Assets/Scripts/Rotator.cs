using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Tooltip("회전 축")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    
    [Tooltip("회전 속도 (도/초)")]
    [SerializeField] private float rotationSpeed = 30f;

    private void Update()
    {
        // Time.deltaTime을 곱해서 프레임 독립적 회전 보장
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}