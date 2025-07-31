using System.Collections;
using UnityEngine;

public class RandomBounce : MonoBehaviour
{
    [SerializeField] float impulseStrength = 5f;
    [SerializeField] float interval = 10f;
    [SerializeField] float maxSpeed = 1f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ApplyRandomImpulse());
    }

    private IEnumerator ApplyRandomImpulse()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            yield return wait;

            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                // 랜덤 방향의 약한 임펄스
                Vector3 dir = Random.onUnitSphere;
                rb.AddForce(dir * impulseStrength, ForceMode.Impulse);
            }
            else
            {
                // 속도 너무 크면 살짝 감속
                rb.linearVelocity *= 0.95f;
            }
        }
    }
}
