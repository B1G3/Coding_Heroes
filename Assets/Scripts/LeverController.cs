using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeverController : MonoBehaviour
{
    [SerializeField] Transform attachTransform;
    Rigidbody rb;
    XRGrabInteractable grab;
    void Awake()
    {
        rb   = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        grab.trackPosition = false;
        grab.trackRotation = false;
        grab.attachTransform = attachTransform;

        grab.selectEntered.AddListener(_ => rb.isKinematic = false);
        grab.selectExited .AddListener(_ => {
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic     = true;
        });
    }
}