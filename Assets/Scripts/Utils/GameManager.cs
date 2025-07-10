using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPathPrefab;
    public static GameManager Instance { get; private set; }
    void Awake() => Instance = this;

    public ITarget GetTarget()
    {
        if(!enemyPathPrefab) return null;
        return enemyPathPrefab.GetComponent<ITarget>();
    }
}
