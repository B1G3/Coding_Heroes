using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// 메뉴 버튼을 눌렀을 때 미리 설정된 오브젝트들을 플레이어 앞에 정렬시키는 스크립트
/// </summary>
public class FollowerAlignment : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField, Tooltip("퀘스트용 플래그")]
    bool isFirstTime = false;
    public static event Action OnFirstAlignment;
    
    [Header("Input Settings")]
    [SerializeField, Tooltip("메뉴 버튼 입력 액션")]
    private InputActionReference menuButtonAction;

    [Header("Target Objects")]
    [SerializeField, Tooltip("정렬할 오브젝트들")]
    private List<Transform> followersToAlign = new List<Transform>();

    [Header("Alignment Settings")]
    [SerializeField, Tooltip("플레이어로부터 정렬될 거리")]
    float m_AlignmentDistance = 2.0f;

    [SerializeField, Tooltip("친구들 사이의 간격")]
    float m_SpacingBetweenFollowers = 1.5f;

    [SerializeField, Tooltip("정렬 시 플레이어로부터의 높이 오프셋")]
    float m_HeightOffset = 0.0f;

    [SerializeField, Tooltip("정렬이 완료되기까지의 시간")]
    float m_AlignmentDuration = 1.0f;

    [SerializeField, Tooltip("정렬 시 사용할 애니메이션 커브")]
    AnimationCurve m_AlignmentCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Debug")]
    [SerializeField, Tooltip("디버그 로그 표시")]
    bool m_ShowDebugLogs = true;

    // 내부 변수들
    Transform m_MainCamera;
    bool m_IsAligning = false;
    Coroutine m_AlignmentCoroutine;
    
    public bool IsAligning { get; private set; }

    /// <summary>
    /// 각 팔로워의 정렬 데이터를 저장하는 구조체
    /// </summary>
    private struct FollowerAlignmentData
    {
        public Transform follower;
        public Vector3 startPosition;
        public Quaternion startRotation;
        public Vector3 targetPosition;
        public Quaternion targetRotation;
        public LazyFollow lazyFollowComponent;
    }
    
    void Start()
    {
        // 메인 카메라 찾기
        if (Camera.main != null)
            m_MainCamera = Camera.main.transform;
        else
            Debug.LogWarning("Main Camera를 찾을 수 없습니다. 오브젝트들이 올바르게 정렬되지 않을 수 있습니다.");
    }

    void OnEnable()
    {
        if (menuButtonAction != null)
        {
            menuButtonAction.action.Enable();
            menuButtonAction.action.performed += OnMenuButtonPressed;
        }
    }

    void OnDisable()
    {
        if (menuButtonAction != null)
        {
            menuButtonAction.action.performed -= OnMenuButtonPressed;
            menuButtonAction.action.Disable();
        }

        // 진행 중인 정렬 중단
        if (m_AlignmentCoroutine != null)
        {
            StopCoroutine(m_AlignmentCoroutine);
            m_AlignmentCoroutine = null;
        }
    }

    /// <summary>
    /// 메뉴 버튼이 눌렸을 때 호출되는 콜백
    /// </summary>
    private void OnMenuButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!m_IsAligning)
            StartAlignment();
        
        if (!isFirstTime)
        {
            isFirstTime = true;
            OnFirstAlignment?.Invoke();
        }
    }

    /// <summary>
    /// 정렬을 시작합니다
    /// </summary>
    [ContextMenu("Start Alignment")]
    public void StartAlignment()
    {
        if (m_MainCamera == null || followersToAlign.Count == 0)
        {
            if (m_ShowDebugLogs)
                Debug.LogWarning("메인 카메라가 없거나 정렬할 오브젝트가 없어서 정렬을 시작할 수 없습니다.");
            return;
        }

        if (m_ShowDebugLogs)
            Debug.Log($"{followersToAlign.Count}개 오브젝트 정렬을 시작합니다.");

        // 이전 정렬이 진행 중이면 중단
        if (m_AlignmentCoroutine != null)
            StopCoroutine(m_AlignmentCoroutine);

        m_IsAligning = true;

        // 정렬 애니메이션 시작
        m_AlignmentCoroutine = StartCoroutine(AlignmentCoroutine());
    }

    /// <summary>
    /// 정렬 애니메이션 코루틴
    /// </summary>
    IEnumerator AlignmentCoroutine()
    {
        // 정렬 데이터 계산
        var alignmentData = CalculateAlignmentData();

        float elapsedTime = 0f;

        while (elapsedTime < m_AlignmentDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / m_AlignmentDuration);
            float curveValue = m_AlignmentCurve.Evaluate(normalizedTime);

            // 각 오브젝트를 목표 위치로 보간
            foreach (var data in alignmentData)
            {
                if (data.follower == null) continue;

                data.follower.position = Vector3.Lerp(data.startPosition, data.targetPosition, curveValue);
                data.follower.rotation = Quaternion.Slerp(data.startRotation, data.targetRotation, curveValue);
            }

            yield return null;
        }

        // 최종 위치로 정확히 설정
        foreach (var data in alignmentData)
        {
            if (data.follower == null) continue;

            data.follower.position = data.targetPosition;
            data.follower.rotation = data.targetRotation;

            // LazyFollow 컴포넌트가 있다면 다시 활성화
            if (data.lazyFollowComponent != null)
            {
                ResetLazyFollowTarget(data.lazyFollowComponent, data.follower.position, data.follower.rotation);
            }
        }

        CompleteAlignment();
    }
    
    void ResetLazyFollowTarget(LazyFollow lazyFollow, Vector3 pos, Quaternion rot)
    {
        try
        {
            // 1) Transform 바로 세팅
            lazyFollow.transform.SetPositionAndRotation(pos, rot);

            var lazyType  = typeof(LazyFollow);
            var vecField  = lazyType.GetField("m_Vector3TweenableVariable",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var rotField  = lazyType.GetField("m_QuaternionTweenableVariable",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var vecTween  = vecField.GetValue(lazyFollow);
            var rotTween  = rotField.GetValue(lazyFollow);

            // 2) 필드에서 꺼낸 오브젝트 타입으로 직접 프로퍼티·메서드 조회
            var vecType   = vecTween.GetType();
            var rotType   = rotTween.GetType();

            var vecProp   = vecType.GetProperty("target",
                BindingFlags.Public | BindingFlags.Instance);
            var rotProp   = rotType.GetProperty("target",
                BindingFlags.Public | BindingFlags.Instance);

            var vecHandle = vecType.GetMethod("HandleTween",
                BindingFlags.Public | BindingFlags.Instance,
                null, new[]{ typeof(float) }, null);
            var rotHandle = rotType.GetMethod("HandleTween",
                BindingFlags.Public | BindingFlags.Instance,
                null, new[]{ typeof(float) }, null);

            // 3) 내부 target 갱신
            vecProp.SetValue(vecTween, pos);
            rotProp.SetValue(rotTween, rot);

            // 4) 즉시 스냅
            vecHandle.Invoke(vecTween, new object[]{ 1f });
            rotHandle.Invoke(rotTween, new object[]{ 1f });

            // 5) 반영 후 Follow 컴포넌트 켜기
            lazyFollow.enabled = true;
        }
        catch(Exception ex)
        {
            Debug.LogException(ex, lazyFollow);
            lazyFollow.enabled = true;
        }
    }

    /// <summary>
    /// 정렬 데이터를 계산합니다
    /// </summary>
    List<FollowerAlignmentData> CalculateAlignmentData()
    {
        var alignmentDataList = new List<FollowerAlignmentData>();

        var cameraForward = m_MainCamera.forward;
        var cameraRight = m_MainCamera.right;
        var cameraPosition = m_MainCamera.position;

        // Y축 회전만 고려 (지면에 평행하게)
        cameraForward.y = 0;
        cameraForward.Normalize();
        cameraRight.y = 0;
        cameraRight.Normalize();

        int followerCount = followersToAlign.Count;
        float totalWidth = (followerCount - 1) * m_SpacingBetweenFollowers;
        float startOffset = -totalWidth * 0.5f;

        for (int i = 0; i < followerCount; i++)
        {
            var followerTransform = followersToAlign[i];
            if (followerTransform == null) continue;

            // 정렬 위치 계산
            float horizontalOffset = startOffset + (i * m_SpacingBetweenFollowers);
            Vector3 alignmentPosition = cameraPosition + 
                                        (cameraForward * m_AlignmentDistance) + 
                                        (cameraRight * horizontalOffset) +
                                        (Vector3.up * m_HeightOffset);

            // 플레이어를 바라보도록 회전 계산
            Vector3 directionToCamera = cameraPosition - alignmentPosition;
            directionToCamera.y = 0;
            float yRotation = Mathf.Atan2(directionToCamera.x, directionToCamera.z) * Mathf.Rad2Deg + 180f;
            yRotation = Mathf.Repeat(yRotation, 360f); // 0-360 범위로 정규화

            Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);

            var lazyFollow = followerTransform.GetComponent<LazyFollow>();
                
            // LazyFollow 컴포넌트가 있다면 잠시 비활성화
            if (lazyFollow != null)
            {
                lazyFollow.enabled = false;
            }

            var data = new FollowerAlignmentData
            {
                follower = followerTransform,
                startPosition = followerTransform.position,
                startRotation = followerTransform.rotation,
                targetPosition = alignmentPosition,
                targetRotation = targetRotation,
                lazyFollowComponent = lazyFollow
            };

            alignmentDataList.Add(data);

            if (m_ShowDebugLogs)
                Debug.Log($"'{followerTransform.name}' 정렬 위치: {alignmentPosition}");
        }

        return alignmentDataList;
    }

    /// <summary>
    /// 정렬을 완료합니다
    /// </summary>
    void CompleteAlignment()
    {
        m_IsAligning = false;
        m_AlignmentCoroutine = null;
            
        if (m_ShowDebugLogs)
            Debug.Log("정렬 완료! 대령했습니다!");
    }
    
    /// <summary>
    /// 개별 오브젝트를 플레이어 앞 중앙에 정렬시킵니다
    /// </summary>
    /// <param name="targetTransform">정렬할 오브젝트</param>
    /// <param name="customDistance">사용자 정의 거리 (기본값: -1로 설정값 사용)</param>
    /// <param name="customHeightOffset">사용자 정의 높이 오프셋 (기본값: float.MinValue로 설정값 사용)</param>
    public void AlignSingleObject(Transform targetTransform, float customDistance = 2f, float customHeightOffset = 0f)
    {
        if (targetTransform == null || m_MainCamera == null)
        {
            if (m_ShowDebugLogs)
                Debug.LogWarning("타겟 오브젝트나 메인 카메라가 없어서 개별 정렬을 할 수 없습니다.");
            return;
        }
        
        IsAligning = true;

        // 사용자 정의 값이 없으면 기본 설정값 사용
        float distance = customDistance > 0 ? customDistance : m_AlignmentDistance;
        float heightOffset = customHeightOffset != float.MinValue ? customHeightOffset : m_HeightOffset;

        StartCoroutine(AlignSingleObjectCoroutine(targetTransform, distance, heightOffset));
    }
    
    /// <summary>
    /// 개별 오브젝트 정렬 코루틴 (플레이어 앞 중앙)
    /// </summary>
    IEnumerator AlignSingleObjectCoroutine(Transform targetTransform, float distance, float heightOffset)
    {
        var cameraForward = m_MainCamera.forward;
        var cameraPosition = m_MainCamera.position;

        // Y축 회전만 고려 (지면에 평행하게)
        cameraForward.y = 0;
        cameraForward.Normalize();

        // 플레이어 앞 중앙에 위치 계산
        Vector3 targetPosition = cameraPosition + (cameraForward * distance) + (Vector3.up * heightOffset);

        // 플레이어를 바라보도록 회전 계산
        Vector3 lookDirection = (cameraPosition - targetPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180f, 0);

        yield return StartCoroutine(AlignSingleObjectCoroutine(targetTransform, targetPosition, targetRotation));
    }
    
    /// <summary>
    /// 개별 오브젝트 정렬 코루틴 (지정된 위치와 회전)
    /// </summary>
    IEnumerator AlignSingleObjectCoroutine(Transform targetTransform, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (targetTransform == null) yield break;

        var lazyFollow = targetTransform.GetComponent<LazyFollow>();
        
        // LazyFollow 컴포넌트가 있다면 잠시 비활성화
        if (lazyFollow != null)
        {
            lazyFollow.enabled = false;
        }

        var startPosition = targetTransform.position;
        var startRotation = targetTransform.rotation;

        float elapsedTime = 0f;

        if (m_ShowDebugLogs)
            Debug.Log($"'{targetTransform.name}' 개별 정렬 시작: {targetPosition}");

        while (elapsedTime < m_AlignmentDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / m_AlignmentDuration);
            float curveValue = m_AlignmentCurve.Evaluate(normalizedTime);

            // 위치와 회전을 보간
            targetTransform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
            targetTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, curveValue);

            yield return null;
        }

        // 최종 위치로 정확히 설정
        targetTransform.position = targetPosition;
        targetTransform.rotation = targetRotation;

        // LazyFollow 컴포넌트가 있다면 다시 활성화
        if (lazyFollow != null)
        {
            lazyFollow.enabled = true;
        }

        if (m_ShowDebugLogs)
            Debug.Log($"'{targetTransform.name}' 개별 정렬 완료!");
        
        IsAligning = false;
    }
}