using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARHomeSpawnTrigger : MonoBehaviour
{
    [SerializeField] private HomeSpawner homeSpawner;
    private GameObject homeObject;
    private bool isSpawned => homeObject;
    
    private void Start()
    {
        if(!homeSpawner)
            homeSpawner = FindAnyObjectByType<HomeSpawner>();
    }

    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += Spawn;
    }
    
    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= Spawn;
    }

    private void Spawn(GameObject homeObject)
    {
        this.homeObject = homeObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawned) return;
        if (!TryGetSpawnSurfaceData(other, out var surfacePosition, out var surfaceNormal))
            return;

        var infinitePlane = new Plane(surfaceNormal, surfacePosition);
        var contactPoint = infinitePlane.ClosestPointOnPlane(transform.position);
        homeSpawner.TrySpawnObject(contactPoint, surfaceNormal);
    }
    
    private bool TryGetSpawnSurfaceData(Collider objectCollider, out Vector3 surfacePosition, out Vector3 surfaceNormal)
    {
        surfacePosition = default;
        surfaceNormal = default;

        var arPlane = objectCollider.GetComponent<ARPlane>();
        if (arPlane == null)
        {
            return false;
        }

        if (arPlane.alignment != PlaneAlignment.HorizontalUp)
        {
            return false;
        }

        surfaceNormal = arPlane.normal;
        surfacePosition = arPlane.center;
        return true;
    }
}
