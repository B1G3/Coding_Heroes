// using UnityEngine;
//
// public class NormalPlacer : IBlockPlacer
// {
//     private readonly GameObject _placementPrefab;
//     private LayerMask ignoreLayers;
//     private GameObject previewInstance;
//     private BlockPreview blockPreview;
//
//     public NormalPlacer(GameObject placementPrefab, LayerMask ignoreLayers)
//     {
//         _placementPrefab = placementPrefab;
//         this.ignoreLayers     = ignoreLayers;
//     }
//
//     public void StartPlacement()
//     {
//         previewInstance = Object.Instantiate(_placementPrefab);
//         blockPreview = previewInstance.GetComponent<BlockPreview>();
//     }
//
//     public void UpdatePreview(Vector3Int gridPos)
//     {
//         if (!previewInstance) return;
//         
//         var canPlace = IsPlaceable(gridPos);
//         previewInstance.transform.position = gridPos;
//         blockPreview.SetValid(canPlace);
//     }
//
//     public void ConfirmPlacement(Vector3Int gridPos)
//     {
//         if (IsPlaceable(gridPos))
//         {
//             var obj = Object.Instantiate(_placementPrefab, gridPos, Quaternion.identity);
//             if (obj.TryGetComponent(out GridNode grid))
//             {
//                 grid.Initialize(gridPos);
//                 Debug.Log($"[Placer] Placed Node at {gridPos}");
//             }
//             else
//             {
//                 Object.Destroy(obj);
//                 Debug.LogError("[Placer] Invalid prefab setup.");
//             }
//         }
//         
//         CancelPlacement();
//     }
//
//     public void CancelPlacement()
//     {
//         if (!previewInstance) return;
//         
//         Object.Destroy(previewInstance);
//         previewInstance = null;
//         blockPreview = null;
//     }
//
//     public bool IsPlaceable(Vector3Int gridPos)
//     {
//         var hits = Physics.OverlapBox(gridPos, Vector3.one * 0.4f);
//
//         foreach (var hit in hits)
//         {
//             if (((1 << hit.gameObject.layer) & ignoreLayers) != 0)
//                 continue;
//             if (hit.gameObject == previewInstance) 
//                 continue;
//             
//             Debug.Log($"[Placer] Blocked by {hit.gameObject.name} at {gridPos}");
//             return false;
//         }
//         
//         return true;
//     }
// }
