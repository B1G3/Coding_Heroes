using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

public class HomeSpawner : MonoBehaviour
{
    [SerializeField]
    Camera m_CameraToFace;
    
    public Camera cameraToFace
    {
        get
        {
            EnsureFacingCamera();
            return m_CameraToFace;
        }
        set => m_CameraToFace = value;
    }
    
    [SerializeField] private GameObject homePrefab;
    [SerializeField] private DialogueSetup dialogueSetup;
    
    public static event Action<GameObject> OnHomeSpawned;
    
    void EnsureFacingCamera()
    {
        if (m_CameraToFace == null)
            m_CameraToFace = Camera.main;
    }
    
    public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        // 예외처리
        var home = Instantiate(homePrefab);
        
        home.transform.position = spawnPoint;
        
        EnsureFacingCamera();

        var facePosition = m_CameraToFace.transform.position;
        var forward = facePosition - spawnPoint;
        BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
        home.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);
        
        OnHomeSpawned?.Invoke(home);
        home.GetComponent<Home>()?.Initialize();
        
        return true;
    }
}
