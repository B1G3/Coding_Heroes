using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/StageConfig")]
public class StageConfig : ScriptableObject
{
    public string stageInfo;
    
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