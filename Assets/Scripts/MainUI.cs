using System.Collections;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField] private FollowerAlignment followerAlignment;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float delay = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(EnableUIAfterAlignment());
    }
    
    private void OnDisable()
    {
        canvas.enabled = false;
    }

    private IEnumerator EnableUIAfterAlignment()
    {
        canvas.enabled = false;
        yield return new WaitForSeconds(delay);
        followerAlignment.AlignSingleObject(transform);
        yield return new WaitUntil(() => !followerAlignment.IsAligning);
        canvas.enabled = true;
    }
}
