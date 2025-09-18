using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Templates.MR;

public class PassthroughSetup : MonoBehaviour
{
    [SerializeField] private bool planeExist = true;
    
    [SerializeField]
    FadeMaterial m_FadeMaterial;
    
    [SerializeField]
    ARFeatureController m_FeatureController;
    public static event Action OnPlaneExist;
    
    void Start()
    {
        if (m_FadeMaterial != null)
            m_FadeMaterial.FadeSkybox(true);
        StartCoroutine(OnStartGame());
    }

    private void OnEnable()
    {
        MainUiManager.OnLookAroundComplete += OnLookAroundComplete;
        HomeSpawner.OnHomeSpawned += TurnOffPlanes;
    }
    
    private void OnDisable()
    {
        MainUiManager.OnLookAroundComplete -= OnLookAroundComplete;
        HomeSpawner.OnHomeSpawned -= TurnOffPlanes;
    }

    private IEnumerator OnStartGame()
    {
        yield return new WaitForSeconds(1f);
        if (planeExist)
        {
            OnPlaneExist?.Invoke();
        }
    }

    private void OnLookAroundComplete(bool visible)
    {
        if (visible) TurnOnPlanes(true);
    }

    private void TurnOnPlanes(bool visualize)
    {
        if (m_FeatureController != null)
        {
            m_FeatureController.TogglePlanes(true);
            m_FeatureController.TogglePlaneVisualization(true);
        }
    }

    private void TurnOffPlanes(GameObject home)
    {
        if (m_FeatureController != null)
        {
            m_FeatureController.TogglePlanes(false);
        }
    }
}
