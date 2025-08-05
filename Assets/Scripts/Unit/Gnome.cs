using UnityEngine;
using Cysharp.Threading.Tasks;

public class Gnome : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    public bool IsMoving => isMoving;
    
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
        MoveToTargetAsync().Forget();
    }
    
    private async UniTaskVoid MoveToTargetAsync()
    {
        while (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
        
        isMoving = false;
    }
    
    public void StopMoving()
    {
        isMoving = false;
    }
}