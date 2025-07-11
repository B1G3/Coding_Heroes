using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable), typeof(BoxCollider))]
public class ARBoxSpawner : MonoBehaviour
{
    [Header("UI Box Interactable")]
    public XRGrabInteractable uiBox;

    [Header("Prefab to Spawn on AR Plane")]  
    public GameObject boxPrefab;
    
    public float cellSize = 0.2f;
    
    [SerializeField] private TMP_Text text;

    private GameObject currentClone;

    void Awake()
    {
        uiBox ??= GetComponent<XRGrabInteractable>();

        uiBox.selectEntered.AddListener(OnGrab);
        uiBox.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        text.text = "Grab";
        currentClone = Instantiate(boxPrefab);
        currentClone.transform.position = args.interactorObject.transform.position;
        currentClone.transform.rotation = Quaternion.identity;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        text.text = "Release";
        var rig = currentClone.GetComponent<Rigidbody>();
        rig.useGravity = true;
        // if (currentClone == null)
        //     return;
        //
        // // 복제된 오브젝트가 Plane 위에 있는지 확인
        // // if (IsOverPlane(currentClone.transform))
        // // {
        //     Pose hitPose = hits[0].pose;
        //     Vector3 worldPos = hitPose.position;
        //
        //     // 그리드 스냅: gridOrigin 기준 또는 월드 기준
        //     Vector3 localPos = gridOrigin != null
        //         ? gridOrigin.InverseTransformPoint(worldPos)
        //         : worldPos;
        //     int gx = Mathf.RoundToInt(localPos.x / cellSize);
        //     int gz = Mathf.RoundToInt(localPos.z / cellSize);
        //     Vector3 spawnPos = (gridOrigin != null)
        //         ? gridOrigin.TransformPoint(new Vector3(gx * cellSize, worldPos.y, gz * cellSize))
        //         : new Vector3(gx * cellSize, worldPos.y, gz * cellSize);
        //
        //     currentClone.transform.position = spawnPos;
        // // }
        // // else
        // // {
        // //     Destroy(currentClone);
        // // }
        //
        // currentClone = null;
        // hits.Clear();
    }
}
