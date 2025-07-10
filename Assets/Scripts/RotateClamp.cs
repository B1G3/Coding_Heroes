using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RotateClamp : MonoBehaviour
{
    private Transform m_Transform;
    private float duration = 0.5f;
    
    private Coroutine rotateCoroutine;
    
    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
    }
    public void OnSelectEntered(HoverEnterEventArgs args)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateZRelative(90f, duration));
    }

    public void OnSelectExited(HoverExitEventArgs args)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateZRelative(-90f, duration));
    }
    
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateZRelative(90f, duration));
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateZRelative(-90f, duration));
    }
    
    private IEnumerator RotateZRelative(float angle, float duration)
    {
        float elapsed = 0f;
        Quaternion startRot = m_Transform.rotation;
        Quaternion endRot   = startRot * Quaternion.Euler(0, 0, angle);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            m_Transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        m_Transform.rotation = endRot;
        rotateCoroutine = null;
    }
}
