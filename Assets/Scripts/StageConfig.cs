using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/StageConfig")]
public class StageConfig : ScriptableObject
{
    public string stageInfo;

    [Header("Descriptions")]
    public List<DescriptionEntry> descriptions;
    
    [System.Serializable]
    public class DescriptionEntry
    {
        [TextArea] 
        public string text;           // 대사나 설명 텍스트
        public AudioClip audioClip;   // 해당 텍스트의 오디오 클립
    }
    
    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;  // 더미 혹은 목표 오브젝트
        public int count;          // 개수
    }

    [Header("Dummy Objects")]
    public List<SpawnEntry> dummyEntries;

    [Header("Target Objects")]
    public List<SpawnEntry> targetEntries;
}