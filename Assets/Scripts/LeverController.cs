using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverController : MonoBehaviour
{
    private HingeJoint hinge;

    [Header("Return Settings")]
    [Tooltip("돌아올 때 초당 회전할 각속도(도/초)")]
    [SerializeField] float returnSpeed = 180f;

    float   minAngle;
    float   maxAngle;
    bool    isGrabbed;
    bool    maxReached;
    Coroutine returnRoutine;
    
    public static event Action OnLeverMax;

    void Awake()
    {
        hinge = GetComponent<HingeJoint>();

        // 힌지의 limits.min 을 저장
        minAngle = hinge.limits.min;
        maxAngle = hinge.limits.max;

        // 시작 시에도 min 각도로 세팅
        SetLeverAngle(maxAngle);
    }
    
    void Update()
    {
        if (isGrabbed && !maxReached)
        {
            float angle = transform.localRotation.eulerAngles.x;
            // 오차범위 0.5도 이내면 max 도달로 간주
            if (Mathf.Abs(Mathf.DeltaAngle(angle, maxAngle)) < 0.5f)
            {
                maxReached = true;
                OnLeverMax?.Invoke();
            }
        }
    }
    
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        isGrabbed   = true;
        maxReached  = false;
        // 혹시 돌아오는 중이면 멈추기
        if (returnRoutine != null) StopCoroutine(returnRoutine);
        hinge.useMotor = false;
    }
    
    public void OnSelectExit(SelectExitEventArgs args)
    {
        isGrabbed = false;
        
        // 이미 진행 중인 복귀 코루틴이 있으면 중단
        if (returnRoutine != null) 
            StopCoroutine(returnRoutine);

        // minAngle 까지 부드럽게 돌아오는 코루틴 시작
        returnRoutine = StartCoroutine(ReturnToMinAngle());
    }

    private IEnumerator ReturnToMinAngle()
    {
        // 현재 각
        float current = transform.localRotation.eulerAngles.x;
        // 목표값 = minAngle
        float target = maxAngle;
        
        // 목표각과 차이가 0.5도 이하가 될 때까지 반복
        while (Mathf.Abs(Mathf.DeltaAngle(current, target)) > 0.5f)
        {
            // 각을 조금씩 이동
            current = Mathf.MoveTowardsAngle(
                current,
                target,
                returnSpeed * Time.deltaTime
            );
            SetLeverAngle(current);
            yield return null;
        }

        // 정확하게 맞춰주기
        SetLeverAngle(target);
    }

    private void SetLeverAngle(float angle)
    {
        // hinge.axis 를 기준으로 회전
        // 예: axis=(1,0,0)이면 X축 회전
        transform.localRotation = Quaternion.AngleAxis(angle, hinge.axis);
    }
}