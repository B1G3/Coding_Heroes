using UnityEngine;
public class InvertSphere : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f;
    
    void Start()
    {
        var mf = GetComponent<MeshFilter>();
        var mesh = Instantiate(mf.sharedMesh);
        
        // 1) 삼각형 순서 뒤집기
        var tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            (tris[i + 1], tris[i + 2]) = (tris[i + 2], tris[i + 1]);
        }
        mesh.triangles = tris;
        
        // 2) 노멀 뒤집기
        var norms = mesh.normals;
        for (int i = 0; i < norms.Length; i++)
            norms[i] = -norms[i];
        mesh.normals = norms;
        
        mf.mesh = mesh;
        
        var mc = GetComponent<MeshCollider>();
        if (mc == null) mc = gameObject.AddComponent<MeshCollider>();

        // 그대로 쓰면 안쪽에서 밖으로 나갈 때 잘 막히는지 확인.
        mc.sharedMesh = mf.sharedMesh;
        mc.convex = false; // 내부 경계로 쓰려면 false, 이 오브젝트는 움직이면 안 됨 (static)
        mc.isTrigger = false;
    }
}