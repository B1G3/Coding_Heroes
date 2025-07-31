using System.Collections.Generic;
using UnityEngine;

public class PreviewPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private readonly Queue<GameObject> pool = new();

    public GameObject Get()
    {
        // 풀에 남은 애가 있으면 꺼내서 활성화
        if (pool.Count > 0)
        {
            var go = pool.Dequeue();
            go.SetActive(true);
            return go;
        }
        // 없으면 새로 생성
        return Instantiate(prefab);
    }

    public void Release(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }
}