using System.Collections.Generic;
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

    [Header("AR Foundation Components")]
    public ARRaycastManager raycastManager;
    [Tooltip("그리드 좌표 기준이 되는 Transform (예: ARSessionOrigin)")]
    public Transform gridOrigin;
    public float cellSize = 0.2f;

    private GameObject currentClone;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private BoxCollider boxCollider;

    void Awake()
    {
        uiBox ??= GetComponent<XRGrabInteractable>();
        boxCollider = GetComponent<BoxCollider>();

        uiBox.selectEntered.AddListener(OnGrab);
        uiBox.selectExited.AddListener(OnRelease);
    }

    private bool IsOverPlane(Transform checkTransform)
    {
        // 해당 Transform 위치에서 아래 방향으로 Raycast
        Vector3 origin = checkTransform.position + Vector3.up * 0.1f;
        Ray ray = new Ray(origin, Vector3.down);
        return raycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // UI Box 위치가 AR Plane 위에 있어야 복사 가능
        if (!IsOverPlane(uiBox.transform))
            return;

        // 복제 생성
        currentClone = Instantiate(boxPrefab);
        currentClone.transform.position = args.interactorObject.transform.position;
        currentClone.transform.rotation = Quaternion.identity;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (currentClone == null)
            return;

        // 복제된 오브젝트가 Plane 위에 있는지 확인
        if (IsOverPlane(currentClone.transform))
        {
            Pose hitPose = hits[0].pose;
            Vector3 worldPos = hitPose.position;

            // 그리드 스냅: gridOrigin 기준 또는 월드 기준
            Vector3 localPos = gridOrigin != null
                ? gridOrigin.InverseTransformPoint(worldPos)
                : worldPos;
            int gx = Mathf.RoundToInt(localPos.x / cellSize);
            int gz = Mathf.RoundToInt(localPos.z / cellSize);
            Vector3 spawnPos = (gridOrigin != null)
                ? gridOrigin.TransformPoint(new Vector3(gx * cellSize, worldPos.y, gz * cellSize))
                : new Vector3(gx * cellSize, worldPos.y, gz * cellSize);

            currentClone.transform.position = spawnPos;
        }
        else
        {
            Destroy(currentClone);
        }

        currentClone = null;
        hits.Clear();
    }
}
