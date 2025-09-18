using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class MainUiManager : MonoBehaviour
{
    [Serializable]
    public class TutorialQuest
    {
        public AudioClip voiceClip;
        public Toggle questUI;
    }

    [Header("UI")]
    [SerializeField] Image logoPanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] CanvasGroup tutorialCanvasGroup;
    [SerializeField] GameObject warningPanel;
    [SerializeField] private GameObject BlockUI;
    [SerializeField] private GameObject LeverUI;
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject StageUI;
    [SerializeField] private GameObject PlaneUI;
    [SerializeField] private GameObject PlaneMenuUI;
    

    [Header("Tutorial Settings")]
    [SerializeField] List<TutorialQuest> tutorialQuests;
    [SerializeField] float logoDisplayTime = 3f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip completeVoiceClip;
    [SerializeField] AudioClip warnigClip;
    
    [SerializeField] private float defaultVolume = 1f;
    [SerializeField] private float totalDurationSeconds = 3f; // 전체 지속 시간(3초)
    [SerializeField] private float fadeDuration        = 0.8f; // 마지막 페이드 시간
    [SerializeField] private bool  useUnscaledTime     = true;
    [SerializeField] private bool  stopAfterFade       = true;

    Coroutine _fadeCo;

    private bool planeVisible;

    public static event Action<bool> OnLookAroundComplete;
    public static event Action OnTutorialComplete;
    
    // 외부 트리거용 플래그
    bool onLookAround    = false;
    bool onFirstAlignment = false;
    bool onUIMove        = false;
    bool onButtonPressed = false;

    void Start()
    {
        InitializeUI();
        ShowLogoSequence().Forget();
    }

    void OnEnable()
    {
        PassthroughSetup.OnPlaneExist += OnCompleteLookAround;
        FollowerAlignment.OnFirstAlignment += OnTutorialStartedAlignment;
        StageManager.OnBossStage += Warning;
    }
    
    void OnDisable()
    {
        PassthroughSetup.OnPlaneExist -= OnCompleteLookAround;
        FollowerAlignment.OnFirstAlignment -= OnTutorialStartedAlignment;
        StageManager.OnBossStage -= Warning;
    }

    void InitializeUI()
    {
        logoPanel.enabled = true;
        tutorialPanel.SetActive(false);
        warningPanel.SetActive(false);
        PlaneUI.SetActive(false);
        ToggleGameUI(false);
        foreach (var q in tutorialQuests)
        {
            q.questUI.isOn = false;
        }
    }

    async UniTaskVoid ShowLogoSequence()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(logoDisplayTime));
        await logoPanel.DOFade(0f, 1f).AsyncWaitForCompletion();
        logoPanel.enabled = false;
        
        await RunTutorialSequenceAsync();
    }

    /// <summary>
    /// 단일 메서드에서 0,1,2... 퀘스트를 차례로 실행합니다.
    /// </summary>
    async UniTask RunTutorialSequenceAsync()
    {
        PlaneUI.SetActive(true);
        
        await UniTask.WaitUntil(() => onButtonPressed);
        
        PlaneMenuUI.SetActive(false);
        PlaneUI.SetActive(false);
        tutorialPanel.SetActive(true);
        
        // --- Quest 0: 자동 완료 (1초 대기 후) ---
        PlayVoice(0);
        // await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await UniTask.WaitUntil(() => onLookAround);
        CompleteQuest(0);
        OnLookAroundComplete?.Invoke(planeVisible);

        // --- Quest 1: FollowerAlignment.OnFirstAlignment 이벤트 대기 ---
        ToggleGameUI(true);
        await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration));
        PlayVoice(1);
        await UniTask.WaitUntil(() => onFirstAlignment);
        CompleteQuest(1);

        // --- Quest 2: UI 이동 이벤트 대기 ---
        PlayVoice(2);
        await UniTask.WaitUntil(() => onUIMove);
        CompleteQuest(2);

        // ... 이어서 추가 퀘스트가 있다면 같은 패턴으로 ...
        
        // 끝나면 패널 숨기기
        PlaneUI.SetActive(!planeVisible);
        PlayVoice(completeVoiceClip);
        await UniTask.WaitWhile(() => audioSource.isPlaying);
        
        tutorialCanvasGroup
            .DOFade(0f, 1f)
            .SetDelay(1f)
            .OnComplete(() => tutorialPanel.SetActive(false));
        
        warningPanel.SetActive(true);
        OnTutorialComplete?.Invoke();
        Debug.Log("튜토리얼 전부 완료!");
    }

    private void ToggleGameUI(bool visible)
    {
        BlockUI.SetActive(visible);
        // LeverUI.SetActive(visible);
        MenuUI.SetActive(visible);
        StageUI.SetActive(visible);
    }

    public void SetPlaneVisibility(bool visible)
    {
        planeVisible = visible;
        onButtonPressed = true;
    }

    void PlayVoice(int idx)
    {
        var clip = tutorialQuests[idx].voiceClip;
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    
    void PlayVoice(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void Warning()
    {
        if (!audioSource || !warnigClip) return;

        if (_fadeCo != null) { StopCoroutine(_fadeCo); _fadeCo = null; }

        audioSource.loop   = false;           // 경고음은 루프 금지
        audioSource.volume = defaultVolume;
        audioSource.clip   = warnigClip;
        audioSource.Play();

        // 총 3초 = (대기 구간) + (페이드 구간)
        float fade = Mathf.Min(fadeDuration, totalDurationSeconds);
        float wait = Mathf.Max(0f, totalDurationSeconds - fade);

        _fadeCo = StartCoroutine(FadeOutAfter(audioSource, wait, fade, useUnscaledTime, stopAfterFade));
    }

    IEnumerator FadeOutAfter(AudioSource src, float delay, float duration, bool unscaled, bool stopWhenDone)
    {
        if (unscaled) yield return new WaitForSecondsRealtime(delay);
        else          yield return new WaitForSeconds(delay);

        float start = src.volume;
        float t = 0f;
        while (t < duration && src != null)
        {
            t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            src.volume = Mathf.Lerp(start, 0f, k);
            yield return null;
        }

        if (src != null)
        {
            src.volume = 0f;
            if (stopWhenDone) src.Stop();  // 정확히 totalDurationSeconds 근처에서 정지
        }
    }
    
    void CompleteQuest(int idx)
    {
        tutorialQuests[idx].questUI.isOn = true;
        Debug.Log($"퀘스트 {idx} 완료!");
    }

    // 외부에서 이벤트 발생 시 호출
    public void OnCompleteLookAround()     => onLookAround = true;
    public void OnTutorialStartedAlignment() => onFirstAlignment = true;
    public void CompleteUIMove()            => onUIMove        = true;
}
