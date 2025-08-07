using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [Header("스테이지 목록")]
    [Tooltip("진행될 스테이지들을 순서대로 설정합니다.")]
    [SerializeField] private List<StageConfig> stages = new List<StageConfig>();
    
    [Header("보스 스테이지")]
    [Tooltip("별도로 보스 스테이지 인덱스를 지정하지 않으면 마지막 스테이지를 보스로 간주합니다.")]
    [SerializeField] private int bossStageIndex = -1;

    [Header("UI")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private List<Image> fillImages;
    [SerializeField] private List<Image> circleImages;
    
    [Header("애니메이션 설정")]
    [SerializeField] private float fillDuration       = 0.5f;  // fillAmount 0→1 걸리는 시간
    [SerializeField] private float circleScaleDuration = 0.3f;  // circle 확대/축소 한 사이클 시간
    [SerializeField] private float circleScaleFactor   = 1.3f;
    [SerializeField] private Color  highlightColor     = new Color32(0x66, 0xD4, 0x75, 0xFF);
    
    public static event Action<StageConfig> OnStageChanged;
    public static event Action OnBossStage;
    
    public StageConfig CurrentStage { get; private set; }
    public int CurrentStageIndex { get; private set; } = -1;
    
    // 지금까지 진행된 프로그래스 단계 (0부터 시작)
    private int progressStep = 0;

    private void OnEnable()
    {
        MainUiManager.OnTutorialComplete += NextStage;
    }
    
    private void OnDisable()
    {
        MainUiManager.OnTutorialComplete -= NextStage;
    }
    
    /// <summary>다음 스테이지로 이동</summary>
    public void NextStage()
    {
        NextStageSequence().Forget();
    }
    
    private async UniTask NextStageSequence()
    {
        // 1) 이번 단계 애니메이션만 실행
        if (progressStep < circleImages.Count)
        {
            // fill -> circle
            fillImages[progressStep].fillAmount = 0f;
            await fillImages[progressStep]
                .DOFillAmount(1f, fillDuration)
                .SetEase(Ease.Linear)
                .ToUniTask();

            circleImages[progressStep].color = highlightColor;
            await circleImages[progressStep].rectTransform
                .DOScale(circleScaleFactor, circleScaleDuration)
                .SetLoops(2, LoopType.Yoyo)
                .ToUniTask();
        }
        else if (progressStep == circleImages.Count)
        {
            // 마지막 fill
            fillImages[progressStep].fillAmount = 0f;
            await fillImages[progressStep]
                .DOFillAmount(1f, fillDuration)
                .SetEase(Ease.Linear)
                .ToUniTask();
        }

        // 2) 단계 인덱스 증가
        progressStep++;

        // 3) 실제 스테이지 값도 하나 올리고 이벤트 발행
        SetStage(CurrentStageIndex + 1);
    }
    
    /// <summary>명시적 인덱스로 스테이지 설정</summary>
    private void SetStage(int index)
    {
        if (index < 0 || index >= stages.Count)
        {
            Debug.LogWarning($"StageManager: 유효하지 않은 스테이지 인덱스 {index}");
            return;
        }

        CurrentStageIndex = index;
        CurrentStage = stages[index];
        stageText.text = $"{CurrentStage.stageInfo}";

        // 스테이지 변경 이벤트
        OnStageChanged?.Invoke(CurrentStage);

        // 보스 스테이지 도달 시
        if (index == (bossStageIndex < 0 ? stages.Count - 1 : bossStageIndex))
            OnBossStage?.Invoke();
    }
}
