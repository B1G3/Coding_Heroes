using System.Collections;
using UnityEngine;

public class DataContainer : MonoBehaviour, IAttackable
{
    private Animator animator;
    private DataContainerMover mover;
    private bool isDead = false;
    
    public bool IsDead()
    {
        return isDead;
    }
    
    // ITarget 구현 - Position 프로퍼티 수정
    public Vector3 Position => transform.position;
    
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int DieTriggerHash = Animator.StringToHash("Die");

    private void Awake()
    {
        if (TryGetComponent<Animator>(out var anim))
            animator = anim;
    }

    public void SetMover(DataContainerMover mover)
    {
        this.mover = mover;
    }

    public virtual void OnStop()
    {
        mover?.CancelMovement();
    }
    
    public virtual void OnAttack()
    {
        if (isDead) return; // 이미 죽었으면 무시
        
        isDead = true;
        Debug.Log($"DataContainer {name}: OnAttack 호출됨");
        
        // 1) 이동 취소
        mover?.CancelMovement();
        
        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, false);
            animator.SetTrigger(DieTriggerHash);
            StartCoroutine(DieAfterAnimation());
        }
        else
        {
            // Animator가 없으면 즉시 삭제
            Die();
        }
    }

    private IEnumerator DieAfterAnimation()
    {
        // 현재 재생 중인 상태의 길이만큼 대기
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        Die();
    }
    
    protected virtual void Die()
    {
        // 공통 이펙트나 사운드 등
        Destroy(gameObject);
    }
    
    // 풀로 돌아갈 때 상태 초기화
    public void ResetState()
    {
        isDead = false;
    }
}