using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotateClamp : MonoBehaviour
{
    private Transform m_Transform;
    private float duration = 0.5f;

    private Coroutine rotateCoroutine;
    
    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
    }

    public void OnHoverEnter()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateZRelative(90f, duration));
    }

    public void OnHoverExit()
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
