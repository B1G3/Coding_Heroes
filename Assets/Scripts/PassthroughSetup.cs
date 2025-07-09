using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Templates.MR;

public class PassthroughSetup : MonoBehaviour
{
    [SerializeField]
    FadeMaterial m_FadeMaterial;
    
    [SerializeField]
    ARFeatureController m_FeatureController;
    void Start()
    {
        if (m_FadeMaterial != null)
            m_FadeMaterial.FadeSkybox(true);
        
        StartCoroutine(TurnOnPlanes(true));
    }

    public IEnumerator TurnOnPlanes(bool visualize)
    {
        yield return new WaitForSeconds(1f);

        if (m_FeatureController != null)
        {
            m_FeatureController.TogglePlaneVisualization(visualize);
            m_FeatureController.TogglePlanes(true);
        }
    }
}
