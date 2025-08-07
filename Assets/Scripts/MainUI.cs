using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField] private FollowerAlignment followerAlignment;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private float warningBlinkInterval = 1f;

    private void OnEnable()
    {
        StartCoroutine(EnableUIAfterAlignment());
        MainUiManager.OnTutorialComplete += DisableCanvas;
        StageManager.OnBossStage += OnDisplayWarningEvent;
    }
    
    private void OnDisable()
    {
        MainUiManager.OnTutorialComplete -= DisableCanvas;
        StageManager.OnBossStage -= OnDisplayWarningEvent;
    }

    private IEnumerator EnableUIAfterAlignment()
    {
        canvas.enabled = false;
        yield return new WaitForSeconds(delay);
        followerAlignment.AlignSingleObject(transform);
        yield return new WaitUntil(() => !followerAlignment.IsAligning);
        canvas.enabled = true;
    }
    
    private void OnDisplayWarningEvent()
    {
        // 경고 시퀀스를 백그라운드로 실행
        WarningSequenceAsync().Forget();
    }
    
    private async UniTask WarningSequenceAsync()
    {
        ShowWarning();
        await UniTask.Delay(System.TimeSpan.FromSeconds(warningDuration));
        HideWarning();
    }
    
    private void ShowWarning()
    {
        canvas.enabled = true;
        canvasGroup.alpha = 1f;   // 완전 표시 상태 시작
        // DOTween 으로 알파를 0까지 줄였다 다시 돌리는 Yoyo 무한 루프
        canvasGroup
            .DOFade(0f, warningBlinkInterval)
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    private void HideWarning()
    {
        // 트윈 애니메이션 모두 정리
        canvasGroup.DOKill();
        canvasGroup
            .DOFade(0f, delay)
            .OnComplete(() => canvas.enabled = false);
    }
    
    private void DisableCanvas()
    {
        canvas.enabled = false;
    }
}
