using System.Collections.Generic;
using UnityEngine;

public class GnomePool : MonoBehaviour
{
    [SerializeField] private GameObject gnomePrefab;
    [SerializeField] private int initialPoolSize = 10;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    private static GnomePool instance;
    
    public static GnomePool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GnomePool>();
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePool();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject gnome = Instantiate(gnomePrefab);
            gnome.SetActive(false);
            pool.Enqueue(gnome);
        }
    }
    
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject gnome = pool.Dequeue();
            gnome.SetActive(true);
            return gnome;
        }
        else
        {
            return Instantiate(gnomePrefab);
        }
    }
    
    public void Return(Gnome gnome)
    {
        if (gnome == null) return;
        gnome.gameObject.SetActive(false);
        pool.Enqueue(gnome.gameObject);
    }
}