using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MainUiManager : MonoBehaviour
{
    [Serializable]
    public class TutorialQuest
    {
        public AudioClip voiceClip;
        public GameObject questUI;
    }

    [Header("UI")]
    [SerializeField] Image logoPanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] TMP_Text testText;

    [Header("Tutorial Settings")]
    [SerializeField] List<TutorialQuest> tutorialQuests;
    [SerializeField] float logoDisplayTime = 3f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip completeVoiceClip;

    public static event Action OnLookAroundComplete;
    public static event Action OnTutorialComplete;
    
    // 외부 트리거용 플래그
    bool onLookAround    = false;
    bool onFirstAlignment = false;
    bool onUIMove        = false;

    void Start()
    {
        InitializeUI();
        ShowLogoSequence().Forget();
    }

    void OnEnable()
    {
        PassthroughSetup.OnPlaneExist += OnCompleteLookAround;
        FollowerAlignment.OnFirstAlignment += OnTutorialStartedAlignment;
    }
    
    void OnDisable()
    {
        PassthroughSetup.OnPlaneExist -= OnCompleteLookAround;
        FollowerAlignment.OnFirstAlignment -= OnTutorialStartedAlignment;
    }

    void InitializeUI()
    {
        logoPanel.enabled = true;
        tutorialPanel.SetActive(false);
        foreach (var q in tutorialQuests)
        {
            if (q.questUI == null) continue;
            q.questUI.SetActive(false);
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
        tutorialPanel.SetActive(true);
        
        // --- Quest 0: 자동 완료 (1초 대기 후) ---
        ShowQuestUI(0);
        PlayVoice(0);
        // await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await UniTask.WaitUntil(() => onLookAround);
        CompleteQuest(0);
        OnLookAroundComplete?.Invoke();

        // --- Quest 1: FollowerAlignment.OnFirstAlignment 이벤트 대기 ---
        ShowQuestUI(1);
        PlayVoice(1);
        await UniTask.WaitUntil(() => onFirstAlignment);
        CompleteQuest(1);

        // --- Quest 2: UI 이동 이벤트 대기 ---
        ShowQuestUI(2);
        PlayVoice(2);
        await UniTask.WaitUntil(() => onUIMove);
        CompleteQuest(2);

        // ... 이어서 추가 퀘스트가 있다면 같은 패턴으로 ...
        
        // 끝나면 패널 숨기기
        PlayVoice(completeVoiceClip);
        tutorialPanel.SetActive(false);
        Debug.Log("튜토리얼 전부 완료!");
    }

    void ShowQuestUI(int idx)
    {
        testText.text = $"{idx}";
        
        // 이전 UI들 끄고
        foreach (var q in tutorialQuests)
        {
            if (q.questUI == null) continue;
            q.questUI.SetActive(false);
        }
        // 이번 것만 켜기
        if (tutorialQuests[idx].questUI == null) return;
        tutorialQuests[idx].questUI?.SetActive(true);
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

    void CompleteQuest(int idx)
    {
        if (tutorialQuests[idx].questUI == null) return;
        tutorialQuests[idx].questUI.SetActive(false);
        Debug.Log($"퀘스트 {idx} 완료!");
    }

    // 외부에서 이벤트 발생 시 호출
    public void OnCompleteLookAround()     => onLookAround = true;
    public void OnTutorialStartedAlignment() => onFirstAlignment = true;
    public void CompleteUIMove()            => onUIMove        = true;
}
